﻿using AutoMapper;
using Elfie.Serialization;
using EnglishVibes.API.DTO;
using EnglishVibes.Data.Models;
using EnglishVibes.Infrastructure.Data;
using EnglishVibes.Service.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EnglishVibes.API.Controllers
{

    public class GroupController : BaseAPIController
    {

        private readonly ApplicationDBContext context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> userManager;

        public GroupController(
            ApplicationDBContext _context,
            IMapper mapper,
            UserManager<ApplicationUser> _userManager)
        {
            context = _context;
            _mapper = mapper;
            userManager = _userManager;
        }
        // what should i do :- 

        //1-  Action to return inactive group (level , student in this group [ number , names]) 
        [HttpGet("inactive")]
        public async Task<ActionResult<IEnumerable<InActiveGroupDto>>> GetInActiveGroup()
        {
            var inactiveGroups = await context.Groups
                                              .Where(s => !s.ActiveStatus)
                                              .Include(g=>g.Students)
                                              .ToListAsync();
            var map = _mapper.Map<IEnumerable<Group>, IEnumerable<InActiveGroupDto>>(inactiveGroups);

            return Ok(map.ToList());
        }

        // 2-  Action to return Active group(level , student in this group , [number, names])
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ActiveGroupDto>>> GetActiveGroup()
        {
            var ActiveGroups = await context.Groups
                                            .Where(s => s.ActiveStatus)
                                            .Include(g => g.Students)
                                            .Include(g => g.Instructor)
                                            .Include(g => g.GroupWeekDays)
                                            .ToListAsync();
            var map = _mapper.Map<IReadOnlyList<Group>, IReadOnlyList<ActiveGroupDto>>(ActiveGroups);

            return Ok(map.ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<GroupDto> GetGroupById(int id)
        {
            var Groups = context.Groups.Include(g => g.Students).Include(g => g.Instructor).FirstOrDefault(n => n.Id == id);
            //   var map = _mapper.Map<IReadOnlyList<Group>, IReadOnlyList<ActiveGroupDto>>(ActiveGroups);
            GroupDto Group = new GroupDto()
            {
                Id = Groups.Id,
                Level = Groups.Level,
                StudyPlan = Groups.StudyPlan,
                ActiveStatus = Groups.ActiveStatus,


                //  Students = Groups.Students.Select(g => g.Id).ToList()

            };
            if (Groups.ActiveStatus)
            {
                Group.Instructor.Add(Groups.Instructor.UserName);
            }
            else
            {
                foreach (var instructor in context.Instructors) 
                {
                    Group.Instructor.Add(instructor.UserName);

                }

            }

            foreach (var s in Groups.Students)
            {
                Group.Students.Add(s.FirstName);

            }

            return Ok(Group);
        }



        //3-  Action Complete Group-Data [httpput] (startdate,instructor,timeslot) 
        [HttpPost("{id}")]
        public async Task<ActionResult> CompleteGroupData(int id, DateTime StartDate, Guid instructorId, DateTime TimeSlot, DayOfWeek d1, DayOfWeek d2)
        {
            var group = await context.Groups.FindAsync(id); // we will take group id from form
            group.StartDate = StartDate;
            group.InstructorId = instructorId;
            group.TimeSlot = TimeSlot;
            group.GroupWeekDays.Add(new GroupWeekDays { GroupId = id, WeekDay = d1 });
            group.GroupWeekDays.Add(new GroupWeekDays { GroupId = id, WeekDay = d2 });
            context.Groups.Update(group);
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}
