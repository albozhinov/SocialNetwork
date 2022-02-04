namespace Database.Logic
{

    /// <summary>
    /// Write a static class TagTransofrmer that would have a single public static method inside Transform(string tag). That method would convert given tag to a valid one (remove all spaces, put pound sign at first position if it is not present and reduce the length of the tag if it is more than 20 symbols).
    /// </summary>
    public static class TagTransformer
    {
        public static string Transformer(string tag)
        {
            var result = tag.Replace(" ", string.Empty);

            if (!result.StartsWith("#"))
            {
                result = "#" + result;
            }

            if (result.Length > 20)
            {
                result = result.Substring(0, 20);
            }

            return result;
        }


    }
}
