using System.Collections.Generic;
using UnityEngine;

namespace Eliot.Utility
{
	/// <summary>
	/// Helps work with arrays of Audio Clips
	/// </summary>
	public static class AudioUtility  
	{
		/// <summary>
		/// Assign a random Audio Clip to an Audio Source.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static AudioSource SetRandomClip(this AudioSource source, List<AudioClip> list)
		{
			if (list == null || list.Count == 0) return source;
			source.clip = list[UnityEngine.Random.Range(0, list.Count)];
			return source;
		}
		
		/// <summary>
		/// Assign a random Audio Clip to an Audio Source and play that clip.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="list"></param>
		public static void PlayRandomClip(this AudioSource source, List<AudioClip> list)
		{
			if (list == null || list.Count == 0) return;
			source.clip = list[UnityEngine.Random.Range(0, list.Count)];
			source.Play();
		}
	}
}
