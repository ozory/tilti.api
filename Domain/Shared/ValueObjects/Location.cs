using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using NetTopologySuite.Geometries;

namespace Domain.Shared.ValueObjects;

public record Location(Double Latitude, Double Longitude);
