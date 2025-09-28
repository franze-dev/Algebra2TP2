using CustomMath;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiDiagram
{
    private List<VoronoiRegion> _regions;
    private Bounds _bounds;
    private List<CustomPlane> _boundsPlanes;

    public List<VoronoiRegion> Regions => _regions;

    public List<CustomPlane> BoundsPlanes => _boundsPlanes;

    public VoronoiDiagram(List<Vec3> sites, Vec3 minPoint, Vec3 maxPoint)
    {
        _bounds = new Bounds((minPoint + maxPoint) / 2, maxPoint - minPoint);

        _regions = new List<VoronoiRegion>();

        foreach (var site in sites)
            AddSite(site);

        BuildBorderPlanes();

        BuildRegions();
    }

    private void BuildRegions()
    {
        foreach (var region in _regions)
            BuildRegionFor(region);
    }

    private void BuildRegionFor(VoronoiRegion region)
    {
        foreach (var other in _regions)
        {
            if (region == other)
                continue;

            var bisector = GetBisector(region.Site, other.Site, out var mid);

            if (region.BorderExists(bisector, mid))
                continue;

            region.AddBorder(bisector, mid);
        }

        foreach (var border in _boundsPlanes)
            region.AddBorder(border, border.normal * border.distance);
    }


    /// <summary>
    /// The plane between site1 and site2. It'll face towards site1
    /// </summary>
    /// <param name="site1"></param>
    /// <param name="site2"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    private CustomPlane GetBisector(Vec3 site1, Vec3 site2, out Vec3 mid)
    {
        mid = (site1 + site2) * 0.5f;

        var normal = (site1 - site2).normalized;

        var plane = new CustomPlane(normal, mid);

        return plane;
    }

    private void AddSite(Vec3 site)
    {
        if (!_bounds.Contains(site) && site != _bounds.max && site != _bounds.min)
            return;

        var region = new VoronoiRegion(_bounds, site);

        _regions.Add(region);
    }

    private void BuildBorderPlanes()
    {
        _boundsPlanes = new List<CustomPlane>();

        var min = _bounds.min;
        var max = _bounds.max;
        var center = _bounds.center;

        var leftPlane = new CustomPlane(Vec3.right, new Vec3(min.x, center.y, center.z));
        var rightPlane = new CustomPlane(Vec3.left, new Vec3(max.x, center.y, center.z));
        var bottomPlane = new CustomPlane(Vec3.up, new Vec3(center.x, min.y, center.z));
        var topPlane = new CustomPlane(Vec3.down, new Vec3(center.x, max.y, center.z));
        var backPlane = new CustomPlane(Vec3.forward, new Vec3(center.x, center.y, min.z));
        var frontPlane = new CustomPlane(Vec3.back, new Vec3(center.x, center.y, max.z));

        _boundsPlanes.Add(leftPlane);
        _boundsPlanes.Add(rightPlane);
        _boundsPlanes.Add(bottomPlane);
        _boundsPlanes.Add(topPlane);
        _boundsPlanes.Add(backPlane);
        _boundsPlanes.Add(frontPlane);
    }
}
