using EO.Models;
using EO.Services.Group;
using EO.WebContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EO.Controllers
{
    public class GroupController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GroupController(IGroupService groupService, AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _groupService = groupService;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var groups = await _groupService.GetAllGroupsAsync();
            return View(groups);
        }

        public async Task<IActionResult> Details(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            return View(group);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var data = await _groupService.GetFormDataAsync();

            var model = new CreateGroupPageVM
            {
                Group = new CreateGroupVM(),
                Tags = data.Tags,
                Categories = data.Categories,
                Users = data.Users
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateGroupPageVM model)
        {
            if (!ModelState.IsValid)
            {
                var data = await _groupService.GetFormDataAsync();

                model.Tags = data.Tags;
                model.Categories = data.Categories;
                model.Users = data.Users;

                return View(model);
            }

            await _groupService.CreateGroupAsync(model.Group);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(x => x.Id == id);
            if (group == null) return NotFound();

            // 🔥 Fetch existing mappings
            var selectedTags = await (from gm in _context.GroupTagMaps
                                      join t in _context.GroupTag on gm.TagId equals t.Id
                                      where gm.GroupId == id
                                      select new SimpleDto
                                      {
                                          Id = t.TagName,
                                          Label = t.TagName
                                      }).ToListAsync();

            var selectedCategories = await (from cm in _context.GroupCategoryMaps
                                            join c in _context.GroupCategory on cm.CategoryId equals c.Id
                                            where cm.GroupId == id
                                            select new SimpleDto
                                            {
                                                Id = c.CategoryName,
                                                Label = c.CategoryName
                                            }).ToListAsync();

            var selectedChampions = await _context.GroupChampionsMaps
                .Where(x => x.GroupId == id)
                .Select(x => new SimpleDto
                {
                    Id = x.UserId,
                    Label = x.UserId
                }).ToListAsync();

            var selectedCoordinators = await _context.GroupCoordinatorsMaps
                .Where(x => x.GroupId == id)
                .Select(x => new SimpleDto
                {
                    Id = x.UserId,
                    Label = x.UserId
                }).ToListAsync();

            var vm = new EditGroupPageVM
            {
                Group = new CreateGroupVM
                {
                    Id = group.Id,
                    GroupName = group.GroupName,
                    Tagline = group.Tagline,
                    CoverImage = group.CoverImage,
                    WhatsappLink = group.WhatsappLink,
                },

                SelectedTags = selectedTags,
                SelectedCategories = selectedCategories,
                SelectedChampions = selectedChampions,
                SelectedCoordinators = selectedCoordinators,

                Tags = await _context.GroupTag.ToListAsync(),
                Categories = await _context.GroupCategory.ToListAsync(),

                Users = await _userManager.Users
                    .Select(u => new UserVM
                    {
                        UserId = u.Id,
                        Name = u.UserName
                    }).ToListAsync()
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditGroupPageVM model)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(x => x.Id == model.Group.Id);

            if (group == null)
                return NotFound();

            // ---------------- BASIC UPDATE ----------------
            group.GroupName = model.Group.GroupName;
            group.Tagline = model.Group.Tagline;
            group.CoverImage = model.Group.CoverImage;
            group.WhatsappLink = model.Group.WhatsappLink;

            // ---------------- REMOVE OLD MAPPINGS ----------------
            _context.GroupTagMaps.RemoveRange(_context.GroupTagMaps.Where(x => x.GroupId == group.Id));
            _context.GroupCategoryMaps.RemoveRange(_context.GroupCategoryMaps.Where(x => x.GroupId == group.Id));
            _context.GroupChampionsMaps.RemoveRange(_context.GroupChampionsMaps.Where(x => x.GroupId == group.Id));
            _context.GroupCoordinatorsMaps.RemoveRange(_context.GroupCoordinatorsMaps.Where(x => x.GroupId == group.Id));

            await _context.SaveChangesAsync();

            // ---------------- TAGS ----------------
            foreach (var tagName in model.Group.Tags ?? new List<string>())
            {
                var tag = await _context.GroupTag.FirstOrDefaultAsync(x => x.TagName == tagName);

                if (tag == null)
                {
                    tag = new GroupTag { TagName = tagName };
                    _context.GroupTag.Add(tag);
                    await _context.SaveChangesAsync();
                }

                _context.GroupTagMaps.Add(new GroupTagMap
                {
                    GroupId = group.Id,
                    TagId = tag.Id
                });
            }

            // ---------------- CATEGORIES ----------------
            foreach (var catName in model.Group.CategoryName ?? new List<string>())
            {
                var cat = await _context.GroupCategory.FirstOrDefaultAsync(x => x.CategoryName == catName);

                if (cat == null)
                {
                    cat = new GroupCategory { CategoryName = catName };
                    _context.GroupCategory.Add(cat);
                    await _context.SaveChangesAsync();
                }

                _context.GroupCategoryMaps.Add(new GroupCategoryMap
                {
                    GroupId = group.Id,
                    CategoryId = cat.Id
                });
            }

            // ---------------- CHAMPIONS ----------------
            foreach (var userId in model.Group.ChampionUserIds ?? new List<string>())
            {
                _context.GroupChampionsMaps.Add(new GroupChampionsMap
                {
                    GroupId = group.Id,
                    UserId = userId
                });
            }

            // ---------------- COORDINATORS ----------------
            foreach (var userId in model.Group.CoordinatorUserIds ?? new List<string>())
            {
                _context.GroupCoordinatorsMaps.Add(new GroupCoordinatorsMap
                {
                    GroupId = group.Id,
                    UserId = userId
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            await _groupService.DeleteGroupAsync(id);
            return RedirectToAction("Index");
        }
    }
}
