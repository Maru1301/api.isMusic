﻿using api.iSMusic.Models.DTOs;
using api.iSMusic.Models.EFModels;
using api.iSMusic.Models.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.iSMusic.Models.Infrastructures.Repositories
{
	public class MemberRepository: IRepository, IMemberRepository
	{
		private readonly AppDbContext _db;

		public MemberRepository(AppDbContext db)
		{
			_db = db;
		}

		public Member? GetMemberById(int memberId)
		{
			return _db.Members.SingleOrDefault(m => m.Id == memberId);
		}

		public async Task<Member?> GetMemberAsync(int memberId)
		{
			return await _db.Members.SingleOrDefaultAsync(m => m.Id == memberId);
		}

		public void AddLikedSong(int memberId, int songId)
		{
			var data = new LikedSong
			{
				MemberId = memberId,
				SongId= songId,
				Created= DateTime.Now,
			};

			_db.LikedSongs.Add(data);
			_db.SaveChanges();
		}

		public void DeleteLikedSong(int memberId, int songId)
		{
			var data = _db.LikedSongs.SingleOrDefault(ls => ls.MemberId== memberId && ls.SongId == songId);

			if (data == null) throw new Exception("此歌曲不在喜歡列表內");

			_db.LikedSongs.Remove(data);
			_db.SaveChanges();
		}

		public void AddLikedPlaylist(int memberId, int playlistId)
		{
			var data = new LikedPlaylist
			{
				MemberId = memberId,
				PlaylistId = playlistId,
				Created = DateTime.Now,
			};

			_db.LikedPlaylists.Add(data);
			_db.SaveChanges();
		}

		public void DeleteLikedPlaylist(int memberId, int playlistId)
		{
			var data = _db.LikedPlaylists.SingleOrDefault(lp => lp.MemberId == memberId && lp.PlaylistId == playlistId);

			if (data == null) throw new Exception("此歌曲不在喜歡列表內");

			_db.LikedPlaylists.Remove(data);
			_db.SaveChanges();
		}

		public void AddLikedAlbum(int memberId, int albumId)
		{
			var data = new LikedAlbum
			{
				MemberId = memberId,
				AlbumId = albumId,
				Created = DateTime.Now,
			};

			_db.LikedAlbums.Add(data);
			_db.SaveChanges();
		}

		public void DeleteLikedAlbum(int memberId, int albumId)
		{
			var data = _db.LikedAlbums.SingleOrDefault(la => la.MemberId == memberId && la.AlbumId == albumId);

			if (data == null) throw new Exception("此歌曲不在喜歡列表內");

			_db.LikedAlbums.Remove(data);
			_db.SaveChanges();
		}

		public void FollowArtist(int memberId, int artistId)
		{
			var data = new ArtistFollow
			{
				MemberId = memberId,
				ArtistId = artistId,
				Created = DateTime.Now,
			};

			_db.ArtistFollows.Add(data);
			_db.SaveChanges();
		}

		public void UnfollowArtist(int memberId, int artistId)
		{
			var data = _db.ArtistFollows.SingleOrDefault(af => af.MemberId == memberId && af.ArtistId == artistId);

			if (data == null) throw new Exception("此歌曲不在喜歡列表內");

			_db.ArtistFollows.Remove(data);
			_db.SaveChanges();
		}
	}
}
