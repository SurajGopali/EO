using EO.Models;
using EO.Services.Event;
using EO.WebContext;
using Microsoft.EntityFrameworkCore;

public class EventService : IEventService
{
    private readonly AppDbContext _context;

    public EventService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EventDto>> GetEventsAsync()
    {
        var now = DateTime.UtcNow;

        var events = await _context.Events
            .Include(x => x.EventType)
            .OrderBy(x => x.Date)
            .ThenBy(x => x.StartTime)
            .ToListAsync();

        return events.Select(x =>
        {
            var eventDateTime = x.Date + (x.StartTime ?? TimeSpan.Zero);
            var remainingDays = (eventDateTime - now).TotalDays;

            return new EventDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Location = x.Location,
                Date = x.Date,
                Type = x.EventType?.Name,
                IsRegistered = x.IsRegistered,
                Image = x.Image,
                RemainingDays = remainingDays > 0
                    ? (int)Math.Ceiling(remainingDays)
                    : 0
            };
        }).ToList();
    }

    public async Task<EventDetailsDto> GetEventDetailsAsync(int id)
    {
        var eventData = await _context.Events
            .Include(e => e.EventDetail)
            .Include(e => e.Schedules)
            .Include(e => e.Guests)
                .ThenInclude(eg => eg.User)
                    .ThenInclude(u => u.CompanyDetails)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eventData == null)
            return null;

        return new EventDetailsDto
        {
            Id = eventData.Id.ToString(),
            Title = eventData.EventDetail?.Title,
            Description = eventData.EventDetail?.Description,
            Venue = eventData.EventDetail?.Venue,
            CoverImage = eventData.EventDetail?.CoverImage,
            StartDateTime = eventData.EventDetail?.StartDateTime ?? default,
            EndDateTime = eventData.EventDetail?.EndDateTime ?? default,
            IsRegistered = eventData.IsRegistered ?? false,

            Schedule = eventData.Schedules?
                .OrderBy(s => s.Id)
                .Select(s => new ScheduleDto
                {
                    Id = s.Id,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Title = s.Title,
                    Description = s.Description
                }).ToList() ?? new(),

            Guests = eventData.Guests
    .Where(eg => eg.User != null)
    .Select(eg => new EventGuestDto
    {
        Id = eg.User.Id,
        Name = eg.User.FullName ?? "",
        Avatar = eg.User.ProfileImage ?? "/images/default-user.png",
        Designation = eg.User.CompanyDetails?.Designation ?? "N/A",
        CompanyName = eg.User.CompanyDetails?.CompanyName ?? "N/A"
    })
    .ToList()
        };
    }
    public async Task<CreateEventDetailsDto> GetEventDetailsForEditAsync(int id)
    {
        var e = await _context.Events
            .Include(x => x.EventDetail)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (e?.EventDetail == null)
            return new CreateEventDetailsDto();

        return new CreateEventDetailsDto
        {
            Title = e.EventDetail.Title,
            Description = e.EventDetail.Description,
            Venue = e.EventDetail.Venue,
            CoverImage = e.EventDetail.CoverImage,
            StartDateTime = e.EventDetail.StartDateTime,
            EndDateTime = e.EventDetail.EndDateTime
        };
    }

    public async Task UpsertEventDetailsAsync(int eventId, CreateEventDetailsDto dto)
    {
        var e = await _context.Events
            .Include(x => x.EventDetail)
            .FirstOrDefaultAsync(x => x.Id == eventId);

        if (e == null) return;

        if (e.EventDetail == null)
        {
            e.EventDetail = new EventDetail
            {
                EventId = eventId,
                Title = dto.Title,
                Description = dto.Description,
                Venue = dto.Venue,
                CoverImage = dto.CoverImage,
                StartDateTime = dto.StartDateTime,
                EndDateTime = dto.EndDateTime
            };
        }
        else
        {
            e.EventDetail.Title = dto.Title;
            e.EventDetail.Description = dto.Description;
            e.EventDetail.Venue = dto.Venue;
            e.EventDetail.CoverImage = dto.CoverImage;
            e.EventDetail.StartDateTime = dto.StartDateTime;
            e.EventDetail.EndDateTime = dto.EndDateTime;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<UpdateEventDetailFullDto> GetFullEventForEditAsync(int id)
    {
        var e = await _context.Events
            .Include(x => x.EventDetail)
            .Include(x => x.Schedules)
            .Include(x => x.Guests)
                .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (e == null)
            return null;

        return new UpdateEventDetailFullDto
        {
            EventId = e.Id,

            Event = new CreateEventDetailsDto
            {
                Title = e.EventDetail?.Title,
                Description = e.EventDetail?.Description,
                Venue = e.EventDetail?.Venue,
                CoverImage = e.EventDetail?.CoverImage,
                StartDateTime = e.EventDetail?.StartDateTime ?? default,
                EndDateTime = e.EventDetail?.EndDateTime ?? default
            },

            Schedule = e.Schedules?.Select(s => new ScheduleDto
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                StartTime = s.StartTime,
                EndTime = s.EndTime
            }).ToList() ?? new List<ScheduleDto>(),

            // ✅ FIX HERE
            GuestIds = e.Guests?
                .Select(g => g.UserId)
                .ToList() ?? new List<string>()
        };
    }

    public async Task UpdateFullEventAsync(int id, UpdateEventDetailFullDto dto)
    {
        var e = await _context.Events
            .Include(x => x.EventDetail)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (e == null || dto?.Event == null)
            return;

        // =========================
        // EVENT DETAIL
        // =========================
        if (e.EventDetail == null)
        {
            e.EventDetail = new EventDetail
            {
                EventId = id
            };
            _context.EventDetails.Add(e.EventDetail);
        }

        e.EventDetail.Title = dto.Event.Title;
        e.EventDetail.Description = dto.Event.Description;
        e.EventDetail.Venue = dto.Event.Venue;
        if (!string.IsNullOrEmpty(dto.Event.CoverImage))
        {
            e.EventDetail.CoverImage = dto.Event.CoverImage;
        }
        e.EventDetail.StartDateTime = dto.Event.StartDateTime;
        e.EventDetail.EndDateTime = dto.Event.EndDateTime;

        // =========================
        // SCHEDULE
        // =========================
        var existingSchedules = await _context.EventSchedules
            .Where(x => x.EventId == id)
            .ToListAsync();

        var dtoScheduleIds = dto.Schedule?
            .Where(x => x.Id > 0)
            .Select(x => x.Id)
            .ToList() ?? new List<int>();

        var toDeleteSchedules = existingSchedules
            .Where(x => !dtoScheduleIds.Contains(x.Id))
            .ToList();

        _context.EventSchedules.RemoveRange(toDeleteSchedules);

        if (dto.Schedule != null)
        {
            foreach (var s in dto.Schedule)
            {
                if (s.Id > 0)
                {
                    var existing = existingSchedules.FirstOrDefault(x => x.Id == s.Id);

                    if (existing != null)
                    {
                        existing.Title = s.Title;
                        existing.Description = s.Description;
                        existing.StartTime = s.StartTime;
                        existing.EndTime = s.EndTime;
                    }
                }
                else
                {
                    _context.EventSchedules.Add(new EventSchedule
                    {
                        EventId = id,
                        Title = s.Title,
                        Description = s.Description,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime
                    });
                }
            }
        }

        // =========================
        // GUESTS (FIXED)
        // =========================
        var existingGuests = await _context.EventGuests
    .Where(x => x.EventId == id)
    .ToListAsync();

        var dtoGuestIds = dto.GuestIds ?? new List<string>();

        // DELETE removed
        var toDelete = existingGuests
            .Where(x => !dtoGuestIds.Contains(x.UserId))
            .ToList();

        _context.EventGuests.RemoveRange(toDelete);

        // ADD new
        var existingIds = existingGuests.Select(x => x.UserId).ToList();

        var newGuests = dtoGuestIds
            .Where(x => !existingIds.Contains(x))
            .Select(x => new EventGuest
            {
                EventId = id,
                UserId = x
            });

        await _context.EventGuests.AddRangeAsync(newGuests);

        // =========================
        await _context.SaveChangesAsync();
    }

    public async Task<EventUpdateDto> GetEventForEditAsync(int id)
    {
        var e = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);

        if (e == null)
            return null;

        return new EventUpdateDto
        {
            Title = e.Title,
            Description = e.Description,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            Location = e.Location,
            Date = e.Date,
            TypeId = e.EventTypeId,
            IsRegistered = e.IsRegistered,
            Image = e.Image
        };
    }

    public async Task UpsertEventAsync(int id, EventUpdateDto dto)
    {
        Events e;

        if (id == 0)
        {
            e = new Events();
            _context.Events.Add(e);
        }
        else
        {
            e = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);

            if (e == null)
                return;
        }

        e.Title = dto.Title;
        e.Description = dto.Description;
        e.StartTime = dto.StartTime;
        e.EndTime = dto.EndTime;
        e.Location = dto.Location;
        e.Date = dto.Date;
        e.EventTypeId = dto.TypeId;
        e.IsRegistered = dto.IsRegistered;
        e.Image = dto.Image;

        await _context.SaveChangesAsync();
    }
    public async Task DeleteEventAsync(int id)
    {
        var e = await _context.Events
            .Include(x => x.Schedules)
            .Include(x => x.Guests)
            .Include(x => x.EventDetail)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (e == null)
            return;

        if (e.Schedules != null)
            _context.EventSchedules.RemoveRange(e.Schedules);

        if (e.Guests != null)
            _context.EventGuests.RemoveRange(e.Guests);

        if (e.EventDetail != null)
            _context.EventDetails.Remove(e.EventDetail);

        _context.Events.Remove(e);

        await _context.SaveChangesAsync();
    }

    public async Task<List<EventType>> GetEventTypesAsync()
    {
        return await _context.EventTypes
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
    public async Task<List<EventDto>> FilterEventsAsync(int? eventTypeId, int? year, string? range)
    {
        var today = DateTime.Now.Date;

        var query = _context.Events
            .Include(x => x.EventType)
            .AsQueryable();

        // =====================
        // FILTER: EVENT TYPE
        // =====================
        if (eventTypeId.HasValue)
        {
            query = query.Where(x => x.EventTypeId == eventTypeId.Value);
        }

        // =====================
        // FILTER: YEAR
        // =====================
        if (year.HasValue)
        {
            query = query.Where(x => x.Date.Year == year.Value);
        }

        // =====================
        // FILTER: RANGE
        // =====================
        if (!string.IsNullOrEmpty(range))
        {
            range = range.ToLower();

            if (range == "today")
            {
                query = query.Where(x => x.Date.Date == today);
            }
            else if (range == "week")
            {
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                var endOfWeek = startOfWeek.AddDays(7);

                query = query.Where(x =>
                    x.Date >= startOfWeek &&
                    x.Date < endOfWeek);
            }
            else if (range == "month")
            {
                query = query.Where(x =>
                    x.Date.Month == today.Month &&
                    x.Date.Year == today.Year);
            }
        }

        return await query
            .OrderBy(x => x.Date)
            .ThenBy(x => x.StartTime)
            .Select(x => new EventDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Location = x.Location,
                Date = x.Date,
                TypeId = x.EventTypeId,
                Type = x.EventType != null ? x.EventType.Name : null,
                IsRegistered = x.IsRegistered,
                Image = x.Image
            })
            .ToListAsync();
    }
    public async Task<List<EventDto>> GetUpcomingEventsAsync()
    {
        var today = DateTime.Today;
        var now = DateTime.Now;

        var events = await _context.Events
            .Where(x => x.Date.Date > today)
            .Include(x => x.EventType)
            .OrderBy(x => x.Date)
            .ThenBy(x => x.StartTime)
            .ToListAsync();

        return events.Select(x => new EventDto
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            StartTime = x.StartTime,
            EndTime = x.EndTime,
            Location = x.Location,
            Date = x.Date,
            TypeId = x.EventTypeId,
            Type = x.EventType?.Name,
            IsRegistered = x.IsRegistered,
            Image = x.Image,

            RemainingDays = (int)Math.Ceiling(
                ((x.Date + (x.StartTime ?? TimeSpan.Zero)) - now).TotalDays
            )
        }).ToList();
    }
    public async Task<List<EventDto>> GetTodayEventsAsync()
    {
        var today = DateTime.Today;
        var now = DateTime.Now;

        var events = await _context.Events
            .Where(x => x.Date.Date == today)
            .Include(x => x.EventType)
            .OrderBy(x => x.StartTime)
            .ToListAsync();

        return events.Select(x => new EventDto
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            StartTime = x.StartTime,
            EndTime = x.EndTime,
            Location = x.Location,
            Date = x.Date,
            TypeId = x.EventTypeId,
            Type = x.EventType.Name,
            IsRegistered = x.IsRegistered,
            Image = x.Image,
            RemainingDays = 0 // today
        }).ToList();
    }
}