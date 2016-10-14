namespace X.UI.Consoles
{
    public class Strategy
    {
        public Strategy(int circle1, int circle2)
        {
            Circle1 = circle1;
            Circle2 = circle2;
        }
        
        /// <summary>
        /// 短周期
        /// </summary>
        public int Circle1 { get; set; }

        /// <summary>
        /// 长周期
        /// </summary>
        public int Circle2 { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public Price Price { get; set; }

        /// <summary>
        /// 面积结果
        /// </summary>
        public AreaResult Result { get; set; }

        /// <summary>
        /// 幅度
        /// </summary>
        public Wave Wave { get; set; }
    }
}
