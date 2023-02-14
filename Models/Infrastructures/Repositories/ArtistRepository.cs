﻿using api.iSMusic.Models.DTOs.MusicDTOs;
using api.iSMusic.Models.EFModels;
using api.iSMusic.Models.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using static api.iSMusic.Controllers.MembersController;

namespace api.iSMusic.Models.Infrastructures.Repositories
{
    public class ArtistRepository : IRepository, IArtistRepository
	{
		private readonly AppDbContext _db;

		private readonly int skipNumber = 5;

		private readonly int takeNumber = 5;

		public ArtistRepository(AppDbContext db)
		{
			_db = db;
		}

		public Artist? GetArtistByIdForCheck(int artistId)
		{
			return _db.Artists.Find(artistId);
		}

		public ArtistIndexDTO? GetArtistById(int artistId)
		{
			return _db.Artists
				.Select(artist => new ArtistIndexDTO
				{
					Id = artist.Id,
					ArtistName= artist.ArtistName,
					ArtistPicPath= artist.ArtistPicPath,
				})
				.SingleOrDefault(dto => dto.Id == artistId);
		}

		public IEnumerable<ArtistIndexDTO> GetArtistsByName(string artistName, int skipRows, int takeRows)
		{
			return _db.Artists
				.Where(artist => artist.ArtistName.Contains(artistName))
				.Select(artist => new ArtistIndexDTO
				{
					Id = artist.Id,
					ArtistName = artist.ArtistName,
					ArtistPicPath = artist.ArtistPicPath,
					Follows = artist.ArtistFollows.Count(),
				})
				.OrderBy(dto => dto.Follows)
				.Skip(skipRows)
				.Take(takeRows)
				.ToList();
		}

		public IEnumerable<ArtistIndexDTO> GetLikedArtists(int memberId, LikedQueryBody body)
		{
			var follows = _db.ArtistFollows.Where(follow => follow.MemberId == memberId);
			IEnumerable<Artist> artists = body.Condition switch
			{
				"RecentlyAdded" => follows
										.OrderByDescending(follow => follow.Created)
										.Select(follow => follow.Artist),
				"Alphatically" => follows
										.Select(follows => follows.Artist)
										.OrderBy(artist => artist.ArtistName),
				_ => new List<Artist>(),
			};

			artists = body.RowNumber == 2 ?
				artists.Take(takeNumber * 2) :
				artists.Skip((body.RowNumber - 1) * skipNumber)
				.Take(takeNumber);

			return artists.Select(artist => new ArtistIndexDTO 
			{ 
				Id = artist.Id, 
				ArtistName = artist.ArtistName, 
				ArtistPicPath= artist.ArtistPicPath,
			});
		}
	}
}
