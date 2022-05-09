namespace LibH2A.Common
{

  public static class TypeExtensions
  {

    public static TAttribute GetAttribute<TAttribute>( this Type type, bool inherit = false )
      where TAttribute : Attribute
    {
      return type
        .GetCustomAttributes( typeof( TAttribute ), inherit )
        .Cast<TAttribute>()
        .Single();
    }

  }

}
