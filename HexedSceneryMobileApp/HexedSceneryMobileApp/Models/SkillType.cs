﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedSceneryMobileApp.Models
{
    public class SkillType
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public List<Skill>? Skills { get; set; }
    }
}
