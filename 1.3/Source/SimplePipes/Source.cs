﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace UdderlyEvelyn.SimplePipes
{
    public class Source : ResourceUser, ISource
    {
        protected float _originalResourceTotal;
        protected float _remaining;
        protected bool _limitedAmount;
        protected bool _empty = false;

        public float OriginalResourceTotal
        {
            get => _originalResourceTotal;
            set => _originalResourceTotal = value;
        }

        public float Remaining
        {
            get => _remaining;
            set => _remaining = value;
        }

        public bool LimitedAmount
        {
            get => _limitedAmount;
            set => _limitedAmount = value;
        }

        public bool Empty
        {
            get => _empty;
            set => _empty = value;
        }

        public Source()
        {
            if (LimitedAmount)
                Remaining = OriginalResourceTotal;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _originalResourceTotal, "OriginalResourceTotal");
            Scribe_Values.Look(ref _remaining, "Remaining");
            Scribe_Values.Look(ref _limitedAmount, "LimitedAmount");
            Scribe_Values.Look(ref _empty, "Empty");
        }
    }
}