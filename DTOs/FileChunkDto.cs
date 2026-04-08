namespace _5sAudit.DTOs
{

    public class FileChunkDto
    {
        public Microsoft.AspNetCore.Http.IFormFile Chunk { get; set; }
        public string FileName { get; set; }
        public int ChunkNumber { get; set; }
        public int TotalChunks { get; set; }
        public string FileUid { get; set; }
    }
}