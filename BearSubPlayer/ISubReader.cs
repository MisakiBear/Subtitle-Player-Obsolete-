﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BearSubPlayer
{
    public interface ISubReader
    {
        public List<SubInfo> Read(string path);
        public Task<List<SubInfo>> ReadAsync(string path);
    }
}