﻿using api.iSMusic.Models.DTOs.MusicDTOs;
using api.iSMusic.Models.EFModels;
using api.iSMusic.Models.Services.Interfaces;

namespace api.iSMusic.Models.Infrastructures.Repositories
{
    public class ArtistRepository : IRepository, IArtistRepository
	{
		private readonly AppDbContext _db;

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
	}
}
