namespace X.Interface.Dto.HttpRequest
{
    /// <summary>
    /// 删除Log文件Dto
    /// </summary>
    public class DeleteLogFilesRequestDto : UserRequestDtoBase
    {
        /// <summary>
        /// 保留多长时间的日志，单位是月,(至少为3)
        /// </summary>
        public int Remain { get; set; }
    }
}
