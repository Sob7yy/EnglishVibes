﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishVibes.Service.DTO
{
    public class WaitingListStudentDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string SelectedStudyPlan { get; set; }
    }
}
