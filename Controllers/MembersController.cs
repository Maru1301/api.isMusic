﻿using api.iSMusic.Models;
using api.iSMusic.Models.EFModels;
using api.iSMusic.Models.Infrastructures.Extensions;
using api.iSMusic.Models.Services;
using api.iSMusic.Models.Services.Interfaces;
using api.iSMusic.Models.ViewModels.PlaylistVMs;
using api.iSMusic.Models.ViewModels.QueueVMs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.iSMusic.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class MembersController : ControllerBase
	{
		private readonly IMemberRepository _memberRepository;

		private readonly ISongRepository _songRepository;

		private readonly IPlaylistRepository _playlistRepository;

		private readonly IQueueRepository _queueRepository;

		private readonly MemberService _memberService;

		public MembersController(IMemberRepository memberRepo, ISongRepository songRepository, IPlaylistRepository playlistRepository, IQueueRepository queueRepository)
		{
			_memberRepository = memberRepo;
			_songRepository = songRepository;
			_playlistRepository = playlistRepository;
			_queueRepository = queueRepository;
			_memberService = new (_memberRepository, _playlistRepository, _songRepository);
		}

		[HttpGet]
		[Route("{memberId}/Playlists")]
		public ActionResult<IEnumerable<PlaylistIndexVM>> GetMemberPlaylist([FromRoute] int memberId, [FromQuery] bool myOwn)
		{
			var playlists = _memberService.GetMemberPlaylist(memberId, myOwn);

			if (playlists == null)
			{
				return NotFound("Member not found");
			}

			return Ok(playlists.Select(p => p.ToIndexVM()));
		}

		[HttpGet]
		[Route("{memberId}/Queue")]
		public async Task<IActionResult> GetMemberQueue([FromRoute] int memberId)
		{
			try
			{
				var member = await _memberRepository.GetMemberAsync(memberId);
				if (member == null)
				{
					return NotFound(new { message = "Member not found" });
				}

				var queue = await _queueRepository.GetQueueByMemberIdAsync(memberId);
				if (queue == null)
				{
					return NotFound(new { message = "Queue not found for the given member" });
				}

				return Ok(queue.ToIndexVM());
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpGet]
		[Route("{memberId}/RecentlyPlayed")]
		public IActionResult GetRecentlyPlayed(int memberId)
		{
			//Check if the provided memberAccount is valid
			if (memberId <= 0)
			{
				return BadRequest("Invalid member account");
			}

			var _songService = new SongService(_songRepository, _memberRepository);

			var result = _songService.GetRecentlyPlayed(memberId);

			if (!result.Success)
			{
				return NotFound(result.ErrorMessage);
			}

			return Ok(result.RecentlyPlayedSongs.Select(dto => dto.ToIndexVM()));
		}

		[HttpPost]
		[Route("{memberId}/Playlist")]
		public async Task<IActionResult> CreatePlaylist([FromRoute] int memberId)
		{
			//Check if the provided memberAccount is valid
			if (memberId <= 0)
			{
				return BadRequest("Invalid member account");
			}

			var _playlistService = new PlaylistService(_playlistRepository, _songRepository);

			var playlistId = await _playlistService.CreatePlaylistAsync(memberId);

			//Return a 201 Created status code along with the newly created playlist's information
			return Ok(playlistId);
		}

		[HttpPost]
		[Route("{memberId}/LikedSongs/{songId}")]
		public IActionResult AddLikedSong(int memberId, int songId)
		{
			var result = _memberService.AddLikedSong(memberId, songId);

			if (!result.Success)
			{
				return BadRequest(result.Message);
			}

			return Ok(result.Message);
		}
	}
}
