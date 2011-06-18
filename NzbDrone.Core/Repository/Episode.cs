﻿using System;
using System.Collections.Generic;
using NzbDrone.Core.Model;
using PetaPoco;

namespace NzbDrone.Core.Repository
{
    [TableName("Episodes")]
    [PrimaryKey("EpisodeId", autoIncrement = true)]
    public class Episode
    {

        public virtual int EpisodeId { get; set; }

        public virtual int? TvDbEpisodeId { get; set; }

        public virtual int SeriesId { get; set; }
        public virtual int EpisodeFileId { get; set; }
        public virtual int SeasonNumber { get; set; }
        public virtual int EpisodeNumber { get; set; }
        public virtual string Title { get; set; }
        public virtual DateTime AirDate { get; set; }


        public virtual string Overview { get; set; }

        public virtual Boolean Ignored { get; set; }

        [Ignore]
        public Boolean IsDailyEpisode
        {
            get
            {
                return EpisodeNumber == 0;
            }
        }

        /// <summary>
        /// Gets or sets the grab date.
        /// </summary>
        /// <remarks>
        /// Used to specify when the episode was grapped.
        /// this filed is used by status as an expirable "Grabbed" status.
        /// </remarks>
        public virtual DateTime? GrabDate { get; set; }


        [Ignore]
        public EpisodeStatusType Status
        {
            get
            {
                if (GrabDate != null && GrabDate.Value.AddDays(1) >= DateTime.Now)
                {
                    return EpisodeStatusType.Downloading;
                }

                if (EpisodeFileId != 0) return EpisodeStatusType.Ready;


                if (Ignored) return EpisodeStatusType.Ignored;

                if (AirDate.Date.Year > 1900 && DateTime.Now.Date >= AirDate.Date)
                {
                    return EpisodeStatusType.Missing;
                }

                return EpisodeStatusType.NotAired;
            }
        }


        [Ignore]
        public virtual Series Series { get; set; }


        [Ignore]
        public virtual EpisodeFile EpisodeFile { get; set; }


        [Ignore]
        public virtual IList<History> Histories { get; protected set; }

        public override string ToString()
        {
            var seriesTitle = Series == null ? "[NULL]" : Series.Title;

            if (IsDailyEpisode)
                return string.Format("{0} - {1}", seriesTitle, AirDate.Date);

            return string.Format("{0} - S{1:00}E{2:00}", seriesTitle, SeasonNumber, EpisodeNumber);

        }
    }
}