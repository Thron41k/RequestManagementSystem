﻿using RequestManagement.Common.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Models
{
    public class Commissions : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public int ApproveForActId { get; set; }
        public Driver? ApproveForAct { get; set; } = null;
        public int ApproveForDefectAndLimitId { get; set; }
        public Driver? ApproveForDefectAndLimit { get; set; } = null;
        public int ChairmanId { get; set; }
        public Driver? Chairman { get; set; }
        public int Member1Id { get; set; }
        public Driver? Member1 { get; set; }
        public int Member2Id { get; set; }
        public Driver? Member2 { get; set; }
        public int Member3Id { get; set; }
        public Driver? Member3 { get; set; }
        public int Member4Id { get; set; }
        public Driver? Member4 { get; set; }
    }
}
