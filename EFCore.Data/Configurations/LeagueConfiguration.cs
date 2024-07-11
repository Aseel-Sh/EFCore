﻿using EFcore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Data.Configurations
{
    internal class LeagueConfiguration : IEntityTypeConfiguration<League>
    {
        public void Configure(EntityTypeBuilder<League> builder)
        {
            builder.HasData(
                    new League
                    {
                        Id = 1,
                        Name = "Jamaica Premeire League",
                        
                    },
                    new League
                    {
                        Id = 2,
                        Name = "English Premiere League",
                    },
                    new League
                    {
                        Id = 3,
                        Name = "La Liga",
                    }
                );
        }
    }
}
