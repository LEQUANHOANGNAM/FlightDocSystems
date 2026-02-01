namespace FlightDocSystem.Helper
{
    public class AutoSegmentAllMp4

    {
        public static List<string> SegmentAllMp4(
      string ffmpegPath,
      string videosFolder)
        {
            var processedVideos = new List<string>();

            var mp4Files = Directory.GetFiles(videosFolder, "*.mp4");

            foreach (var mp4File in mp4Files)
            {
                var videoName = Path.GetFileNameWithoutExtension(mp4File);
                var m3u8Path = Path.Combine(videosFolder, $"{videoName}.m3u8");

                // Nếu đã phân rã rồi thì bỏ qua
                if (File.Exists(m3u8Path))
                    continue;

                VideoProcess.ProccessVideo(
                    ffmpegPath,
                    mp4File,
                    videosFolder,
                    $"{videoName}.m3u8"
                );

                processedVideos.Add(videoName);
            }

            return processedVideos;
        }
    }
}
