using FlightDocSystem.Helper;
using Microsoft.AspNetCore.Mvc;

namespace FlightDocSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestVideoController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public TestVideoController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // ===============================
        // 1️⃣ Phân rã TẤT CẢ file .mp4
        // GET: api/testvideo/segment-all
        // ===============================
        [HttpGet("segment-all")]
        public IActionResult FragmentAllMp4()
        {
            var videosPath = Path.Combine(
                _env.ContentRootPath,
                "Uploads",
                "Videos"
            );

            if (!Directory.Exists(videosPath))
                return NotFound("Uploads/Videos folder not found");

            var mp4Files = Directory.GetFiles(videosPath, "*.mp4");
            var processed = new List<string>();

            foreach (var mp4 in mp4Files)
            {
                var videoName = Path.GetFileNameWithoutExtension(mp4);
                var m3u8Path = Path.Combine(videosPath, $"{videoName}.m3u8");

                // Đã phân rã thì bỏ qua
                if (System.IO.File.Exists(m3u8Path))
                    continue;

                VideoProcess.ProccessVideo(
                    @"D:\ffmpeg\bin\ffmpeg.exe",
                    mp4,
                    videosPath,
                    $"{videoName}.m3u8"
                );

                processed.Add(videoName);
            }

            return Ok(new
            {
                Segmented = processed.Count,
                Videos = processed
            });
        }

        // =========================================
        // 2️⃣ Trả playlist m3u8 (URL tuyệt đối)
        // GET: api/testvideo/m3u8/{videoName}
        // =========================================
        [HttpGet("m3u8/{videoName}")]
        public IActionResult GetMasterPlaylist(string videoName)
        {
            var playlistPath = Path.Combine(
                _env.ContentRootPath,
                "Uploads",
                "Videos",
                $"{videoName}.m3u8"
            );

            if (!System.IO.File.Exists(playlistPath))
                return NotFound("Playlist not found");

            var content = System.IO.File.ReadAllText(playlistPath);
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var lines = content.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (line.EndsWith(".ts"))
                {
                    lines[i] =
                        $"{baseUrl}/api/testvideo/segment/{videoName}/{line}";
                }
            }

            var updatedContent = string.Join("\n", lines);
            return Content(updatedContent, "application/x-mpegURL");
        }

        // =========================================
        // 3️⃣ Trả segment
        // GET: api/testvideo/segment/{videoName}/{segmentFile}
        // =========================================
        [HttpGet("segment/{videoName}/{segmentFile}")]
        public IActionResult GetSegment(string videoName, string segmentFile)
        {
            var segmentPath = Path.Combine(
                _env.ContentRootPath,
                "Uploads",
                "Videos",
                segmentFile
            );

            if (!System.IO.File.Exists(segmentPath))
                return NotFound();

            return PhysicalFile(segmentPath, "video/mp2t");
        }

        // =========================================
        // 4️⃣ Debug path (test 1 lần rồi bỏ)
        // GET: api/testvideo/debug-path
        // =========================================
        [HttpGet("debug-path")]
        public IActionResult DebugPath()
        {
            var expectedPath = Path.Combine(
                _env.ContentRootPath,
                "Uploads",
                "Videos"
            );

            return Ok(new
            {
                ContentRootPath = _env.ContentRootPath,
                ExpectedVideosPath = expectedPath,
                Exists = Directory.Exists(expectedPath)
            });
        }
    }
}
