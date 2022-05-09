namespace H2ARipper
{
  public static class ObjectExtensions
  {

    public static T SetValue<T>( this object obj, string fieldName, T value )
    {
      // TODO: This is really slow. Static typing if possible.
      obj.GetType().GetField( fieldName ).SetValue( obj, value );
      return value;
    }

  }
}
