using EO.Models;
using EO.Services.Group;
using EO.WebContext;
using Microsoft.EntityFrameworkCore;

public class GroupService : IGroupService
{
    private readonly AppDbContext _context;

    public GroupService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateGroupAsync(CreateGroupVM request)
    {
        using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            var group = new Group
            {
                GroupName = request.GroupName,
                Tagline = request.Tagline,
                CoverImage = request.CoverImage,
                WhatsappLink = request.WhatsappLink,
                CreatedAt = DateTime.UtcNow
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            // CATEGORY (create if not exists)

            foreach (var categoryName in request.CategoryName.Distinct())
            {
                var category = await _context.GroupCategory
                    .FirstOrDefaultAsync(x => x.CategoryName == categoryName);

                if (category == null)
                {
                    category = new GroupCategory
                    {
                        CategoryName = categoryName
                    };

                    _context.GroupCategory.Add(category);
                    await _context.SaveChangesAsync();
                }

                _context.GroupCategoryMaps.Add(new GroupCategoryMap
                {
                    GroupId = group.Id,
                    CategoryId = category.Id
                });
            }

            // TAGS (create if not exists)
            foreach (var tagName in request.Tags.Distinct())
            {
                var tag = await _context.GroupTag
                    .FirstOrDefaultAsync(x => x.TagName == tagName);

                if (tag == null)
                {
                    tag = new GroupTag
                    {
                        TagName = tagName
                    };

                    _context.GroupTag.Add(tag);
                    await _context.SaveChangesAsync();
                }

                _context.GroupTagMaps.Add(new GroupTagMap
                {
                    GroupId = group.Id,
                    TagId = tag.Id
                });
            }

            // CHAMPIONS
            foreach (var userId in request.ChampionUserIds.Distinct())
            {
                _context.GroupChampionsMaps.Add(new GroupChampionsMap
                {
                    GroupId = group.Id,
                    UserId = userId
                });
            }

            // COORDINATORS
            foreach (var userId in request.CoordinatorUserIds.Distinct())
            {
                _context.GroupCoordinatorsMaps.Add(new GroupCoordinatorsMap
                {
                    GroupId = group.Id,
                    UserId = userId
                });
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return group.Id;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
    public async Task<List<GroupDetailsVM>> GetAllGroupsAsync()
    {
        var groups = await _context.Groups.ToListAsync();

        var result = new List<GroupDetailsVM>();

        foreach (var g in groups)
        {
            var tags = await (from gm in _context.GroupTagMaps
                              join t in _context.GroupTag on gm.TagId equals t.Id
                              where gm.GroupId == g.Id
                              select t.TagName).ToListAsync();

            var category = await (from cm in _context.GroupCategoryMaps
                                  join c in _context.GroupCategory on cm.CategoryId equals c.Id
                                  where cm.GroupId == g.Id
                                  select c.CategoryName).FirstOrDefaultAsync();

            result.Add(new GroupDetailsVM
            {
                Id = g.Id,
                GroupName = g.GroupName,
                Tagline = g.Tagline,
                CoverImage = g.CoverImage,
                WhatsappLink = g.WhatsappLink,
                Category = category,
                Tags = tags
            });
        }

        return result;
    }
    public async Task<GroupDetailsVM> GetGroupByIdAsync(int id)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(x => x.Id == id);

        var tags = await (from gm in _context.GroupTagMaps
                          join t in _context.GroupTag on gm.TagId equals t.Id
                          where gm.GroupId == id
                          select t.TagName).ToListAsync();

        var category = await (from cm in _context.GroupCategoryMaps
                              join c in _context.GroupCategory on cm.CategoryId equals c.Id
                              where cm.GroupId == id
                              select c.CategoryName).FirstOrDefaultAsync();

        var champions = await (from c in _context.GroupChampionsMaps
                               where c.GroupId == id
                               join u in _context.Users on c.UserId equals u.Id
                               select new UserVM
                               {
                                   UserId = u.Id,
                                   Name = u.UserName
                               }).ToListAsync();

        var coordinators = await (from c in _context.GroupCoordinatorsMaps
                                  where c.GroupId == id
                                  join u in _context.Users on c.UserId equals u.Id
                                  select new UserVM
                                  {
                                      UserId = u.Id,
                                      Name = u.UserName
                                  }).ToListAsync();

        return new GroupDetailsVM
        {
            Id = group!.Id,
            GroupName = group.GroupName,
            Tagline = group.Tagline,
            CoverImage = group.CoverImage,
            WhatsappLink = group.WhatsappLink,
            Category = category,
            Tags = tags,
            Champions = champions,
            Coordinators = coordinators
        };
    }

    // =========================
    // FORM DATA (DROPDOWNS)
    // =========================
    public async Task<GroupFormDataVM> GetFormDataAsync()
    {
        return new GroupFormDataVM
        {
            Tags = await _context.GroupTag.ToListAsync(),
            Categories = await _context.GroupCategory.ToListAsync(),
            Users = await _context.Users
                .Select(x => new UserVM
                {
                    UserId = x.Id,
                    Name = x.FirstName + " "+ x.LastName
                }).ToListAsync()
        };
    }
  
    public async Task DeleteGroupAsync(int id)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(x => x.Id == id);

        if (group == null) return;

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();
    }

    public Task<EditGroupVM> GetEditDataAsync(int id)
    {
        throw new NotImplementedException();
    }

    Task<EditGroupPageVM?> IGroupService.GetEditDataAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateGroupAsync(EditGroupVM model)
    {
        throw new NotImplementedException();
    }
}