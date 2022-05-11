namespace LibH2A.Common
{

  public static class NumericExtensions
  {

    public static float ToSNorm( this Int16 value )
      => ( float ) value / Int16.MaxValue;

    public static float ToUNorm( this UInt16 value )
      => ( float ) value / UInt16.MaxValue;

  }

}
