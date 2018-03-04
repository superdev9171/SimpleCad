﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleCAD
{
    public class Dimension : Drawable
    {
        private Point2D p1;
        private Point2D p2;

        public Point2D StartPoint { get => p1; set { p1 = value; NotifyPropertyChanged(); } }
        public Point2D EndPoint { get => p2; set { p2 = value; NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X1 { get { return StartPoint.X; } }
        [Browsable(false)]
        public float Y1 { get { return StartPoint.Y; } }
        [Browsable(false)]
        public float X2 { get { return EndPoint.X; } }
        [Browsable(false)]
        public float Y2 { get { return EndPoint.Y; } }

        private float offset;
        private string str;
        private string fontFamily;
        private FontStyle fontStyle;
        private float textHeight;
        private float scale;
        private int precision;

        public float Offset { get => offset; set { offset = value; NotifyPropertyChanged(); } }
        public string String { get => str; set { str = value; NotifyPropertyChanged(); } }
        public string FontFamily { get => fontFamily; set { fontFamily = value; NotifyPropertyChanged(); } }
        public FontStyle FontStyle { get => fontStyle; set { fontStyle = value; NotifyPropertyChanged(); } }
        public float TextHeight { get => textHeight; set { textHeight = value; NotifyPropertyChanged(); } }
        public float Scale { get => scale; set { scale = value; NotifyPropertyChanged(); } }
        public int Precision { get => precision; set { precision = value; NotifyPropertyChanged(); } }

        public Dimension(Point2D p1, Point2D p2, float textHeight)
        {
            StartPoint = p1;
            EndPoint = p2;

            Offset = 0.4f;
            TextHeight = textHeight;
            String = "<>";
            FontFamily = "Arial";
            FontStyle = FontStyle.Regular;
            Scale = 1;
            Precision = 2;
        }

        public Dimension(float x1, float y1, float x2, float y2, float textHeight)
            : this(new Point2D(x1, y1), new Point2D(x2, y2), textHeight)
        {
            ;
        }

        public override void Draw(DrawParams param)
        {
            GetSubItems().Draw(param);
        }

        public override Extents2D GetExtents()
        {
            float offset = Math.Sign(Offset) * (0.5f * TextHeight + Math.Abs(Offset));

            Vector2D dir = EndPoint - StartPoint;
            float angle = dir.Angle;
            float len = dir.Length;
            Point2D p1 = new Point2D(0, 0);
            Point2D p2 = new Point2D(len, 0);
            Point2D p3 = p1 + new Vector2D(0, offset);
            Point2D p4 = p2 + new Vector2D(0, offset);
            TransformationMatrix2D trans = TransformationMatrix2D.Transformation(1, 1, angle, StartPoint.X, StartPoint.Y);
            p1.TransformBy(trans);
            p2.TransformBy(trans);
            p3.TransformBy(trans);
            p4.TransformBy(trans);

            Extents2D extents = new Extents2D();
            extents.Add(p1);
            extents.Add(p2);
            extents.Add(p3);
            extents.Add(p4);
            return extents;
        }

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            Point2D p1 = StartPoint;
            Point2D p2 = EndPoint;
            p1.TransformBy(transformation);
            p2.TransformBy(transformation);
            StartPoint = p1;
            EndPoint = p2;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            return GetSubItems().Contains(pt, pickBoxSize);
        }

        private Composite GetSubItems()
        {
            Composite items = new Composite();

            float tickSize = 0.5f * TextHeight;

            Vector2D dir = EndPoint - StartPoint;
            float angle = dir.Angle;
            float len = dir.Length;

            // Dimension line
            Line dim = new Line(0, Offset, len, Offset);
            dim.Outline = Outline;
            items.Add(dim);

            // Left tick
            Line tick1 = new Line(0, -tickSize + Offset, 0, tickSize + Offset);
            tick1.Outline = Outline;
            items.Add(tick1);

            // Right tick
            Line tick2 = new Line(len, -tickSize + Offset, len, tickSize + Offset);
            tick2.Outline = Outline;
            items.Add(tick2);

            // Text
            float dist = (StartPoint - EndPoint).Length * Scale;
            string txt = String.Replace("<>", dist.ToString("F" + Precision.ToString()));
            Text textObj = new Text(len / 2, Offset, txt, TextHeight);
            textObj.FontFamily = FontFamily;
            textObj.FontStyle = FontStyle;
            textObj.HorizontalAlignment = StringAlignment.Center;
            textObj.VerticalAlignment = StringAlignment.Center;
            textObj.Outline = Outline;
            items.Add(textObj);

            TransformationMatrix2D trans = TransformationMatrix2D.Transformation(1, 1, angle, StartPoint.X, StartPoint.Y);
            items.TransformBy(trans);

            return items;
        }

        public override ControlPoint[] GetControlPoints(float size)
        {
            return new[]
            {
                new ControlPoint("StartPoint", ControlPoint.ControlPointType.Point, StartPoint, StartPoint),
                new ControlPoint("EndPoint", ControlPoint.ControlPointType.Point, EndPoint, EndPoint),
            };
        }
    }
}
