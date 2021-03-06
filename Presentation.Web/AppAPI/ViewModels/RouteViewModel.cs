﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Web.AppAPI.ViewModels
{
    public class RouteViewModel
    {
        public double TotalDistance { get; set; }
        public virtual ICollection<GPSCoordinateModel> GPSCoordinates { get; set; }
    }
}
