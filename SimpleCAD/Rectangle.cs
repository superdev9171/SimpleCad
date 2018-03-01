﻿using System;
using System.ComponentModel;
using System.Drawing;

namespace SimpleCAD
{
    public class Rectangle : Drawable
    {
        private Point2D center;
        private float width;
        private float height;
        private float rotation;

        public Point2D Center { get { return center; } set { center = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public float Width { get { return width; } set { width = Math.Abs(value); UpdatePolyline(); NotifyPropertyChanged(); } }
        public float Height { get { return height; } set { height = Math.Abs(value); UpdatePolyline(); NotifyPropertyChanged(); } }
        public Point2D Corner
        {
            get
            {
                return new Point2D(Center.X + Width / 2, Center.Y + Height / 2);
            }
            set
            {
                width = Math.Abs(value.X - center.X) * 2;
                height = Math.Abs(value.Y - center.Y) * 2;
                UpdatePolyline();
                NotifyPropertyChanged();
            }
        }
        public float Rotation { get { return rotation; } set { rotation = value; UpdatePolyline(); NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X { get { return Center.X; } }
        [Browsable(false)]
        public float Y { get { return Center.Y; } }

        private Polyline poly;

        public Rectangle(Point2D center, float width, float height, float rotation = 0)
        {
            Center = center;
            Width = width;
            Height = height;
            Rotation = rotation;
            UpdatePolyline();
        }

        public Rectangle(float x, float y, float width, float height, float rotation = 0)
            : this(new Point2D(x, y), width, height, rotation)
        {
            ;
        }

        public Rectangle(Point2D center, Point2D corner, float rotation = 0)
            : this(center, (corner - center).X * 2, (corner - center).Y * 2, rotation)
        {
            ;
        }

        private void UpdatePolyline()
        {
            poly = new Polyline();
            poly.Points.Add(-Width / 2, -Height / 2);
            poly.Points.Add(+Width / 2, -Height / 2);
            poly.Points.Add(+Width / 2, +Height / 2);
            poly.Points.Add(-Width / 2, +Height / 2);
            poly.Closed = true;
            poly.TransformBy(TransformationMatrix2D.Rotation(rotation));
            poly.TransformBy(TransformationMatrix2D.Translation(X, Y));

        }

        public override void Draw(DrawParams param)
        {
            poly.OutlineStyle = OutlineStyle;
            poly.FillStyle = FillStyle;
            poly.Draw(param);
        }

        public override Extents GetExtents()
        {
            return poly.GetExtents();
        }

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            Center.TransformBy(transformation);
            Vector2D dir = Vector2D.FromAngle(Rotation);
            dir.TransformBy(transformation);
            Rotation = dir.Angle;
            UpdatePolyline();
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            return poly.Contains(pt, pickBoxSize);
        }
    }
}
