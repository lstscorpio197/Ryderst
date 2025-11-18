namespace ShopAdmin.WHttpMessage
{
    public class HttpMessageBody
    {
        public object Data { get; set; }

        public string Description { get; set; }
        public string Description2 { get; set; }

        public HttpMessagePagination Pagination { get; set; }

        public HttpMessageNoti MsgNoti { get; set; }

        public HttpMessageBody()
        {
            MsgNoti = new HttpMessageNoti();
        }

        public HttpMessageBody(int numberRowsOnPage)
        {
            Pagination = new HttpMessagePagination(numberRowsOnPage);
            MsgNoti = new HttpMessageNoti();
        }
    }
}