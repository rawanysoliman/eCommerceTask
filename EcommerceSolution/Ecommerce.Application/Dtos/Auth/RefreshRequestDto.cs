﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Auth
{
    public class RefreshRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
