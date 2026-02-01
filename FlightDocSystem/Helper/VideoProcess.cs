using System.Diagnostics;

namespace FlightDocSystem.Helper
{
    public class VideoProcess
    {
        public static string ProccessVideo(string ffmpegPath, string fileMp4Path, string outPutDir, string playListFileName)
        {

            //string ffmpegPath = "path_to_ffmpeg_executable"; // Replace with the path to your FFmpeg executable
            //string inputVideo = "input_video.mp4";
            //string outputDirectory = "output_hls"; // Output directory for HLS segments
            //string playlistPath = "output_playlist.m3u8"; // Output M3U8 playlist file
            // Create the output directory if it doesn't exist
            string outPut = "";
            if (!System.IO.Directory.Exists(outPutDir))
            {
                System.IO.Directory.CreateDirectory(outPutDir);
            }

            // Run FFmpeg command to segment the video
            Process process = new Process();
            process.StartInfo.FileName = ffmpegPath;
            //process.StartInfo.Arguments = $"-i {fileMp4Path} -c:v libx264 -c:a aac -hls_time 10 -hls_list_size 0 -hls_segment_filename {outPutDir}/segment%d.ts {outPutDir}/{playListFileName}";
            process.StartInfo.Arguments = $"-i {fileMp4Path} -c:v libx264 -c:a aac -strict experimental -b:v 1000k -b:a 128k -f hls -hls_time 10 -hls_playlist_type vod {outPutDir}/{playListFileName}";

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            //process.Exited += (sender, e) => { outPut = "Completed"; };
            process.Start();
            //process.WaitForExit();
            //if(outPut == "Completed")
            process.Close();
            return outPut;
        }
        public static int NumberOfSegments(string fileM3U8)
        {
            try
            {
                // Read all lines from the M3U8 file
                string[] lines = File.ReadAllLines(fileM3U8);

                // Filter out lines starting with '#' (comments) and blank lines
                var segmentLines = lines
                    .Where(line => !line.StartsWith("#") && !string.IsNullOrWhiteSpace(line));

                // Count the number of segments
                return segmentLines.Count();

            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}

