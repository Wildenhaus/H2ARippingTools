using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Haus
{

  /// <summary>
  ///   A class that defines assertion macros.
  /// </summary>
  public static class Assertions
  {

    #region Assert Methods

    /// <summary>
    ///   Asserts that an expression is true.
    /// </summary>
    /// <param name="expression">
    ///   The expression to evaluate.
    /// </param>
    [DebuggerHidden]
    public static void Assert( bool expression )
    {
      if ( !expression )
        ThrowAssertionFailedException();
    }

    /// <summary>
    ///   Asserts that an expression is true.
    /// </summary>
    /// <param name="expression">
    ///   The expression to evaluate.
    /// </param>
    /// <param name="reason">
    ///   The reason the expression must be true.
    /// </param>
    [DebuggerHidden]
    public static void Assert( bool expression, string reason )
    {
      if ( !expression )
        ThrowAssertionFailedException( reason );
    }

    /// <summary>
    ///   Asserts that two values are equal.
    /// </summary>
    /// <typeparam name="T">
    ///   The value <see cref="Type" />.
    /// </typeparam>
    /// <param name="actualValue">
    ///   The actual value.
    /// </param>
    /// <param name="expectedValue">
    ///   The expected value.
    /// </param>
    /// <param name="reason">
    ///   The reason the values must be equal.
    /// </param>
    [DebuggerHidden]
    public static void AssertEqual<T>( T actualValue, T expectedValue, string reason = null )
      => Assert( Equals( actualValue, expectedValue ), reason );

    #endregion

    #region Check Methods

    /// <summary>
    ///   Asserts that an expression is true when checks are enabled at build time.
    /// </summary>
    /// <param name="expression">
    ///   The expression to evaluate.
    /// </param>
    [DebuggerHidden]
    public static void Check( bool expression )
    {
#if CHECKS_ENABLED
      if( !expression )
        ThrowAssertionFailedException();
#endif
    }

    /// <summary>
    ///   Asserts that an expression is true when checks are enabled at build time.
    /// </summary>
    /// <param name="expression">
    ///   The expression to evaluate.
    /// </param>
    /// <param name="reason">
    ///   The reason the expression must be true.
    /// </param>
    [DebuggerHidden]
    public static void Check( bool expression, string reason )
    {
#if CHECKS_ENABLED
      if( !expression )
        ThrowAssertionFailedException( reason );
#endif
    }

    #endregion

    #region Fail Methods

    /// <summary>
    ///   Declares a fail condition by throwing an exception.
    /// </summary>
    [DebuggerHidden]
    public static void Fail()
    {
      ThrowAssertionFailedException();
    }

    /// <summary>
    ///   Declares a fail condition by throwing an exception.
    /// </summary>
    /// <param name="reason">
    ///   The reason a fail condition was hit.
    /// </param>
    [DebuggerHidden]
    public static void Fail( string reason )
    {
      ThrowAssertionFailedException( reason );
    }

    /// <summary>
    ///   Declares a fail condition by throwing an exception.
    /// </summary>
    /// <typeparam name="TException">
    ///   The type of <see cref="System.Exception" /> that is being thrown.
    /// </typeparam>
    [DebuggerHidden]
    public static void Fail<TException>()
      where TException : System.Exception, new()
    {
      ThrowGenericException<TException>( new TException() );
    }

    /// <summary>
    ///   Declares a fail condition by throwing an exception.
    /// </summary>
    /// <typeparam name="TException">
    ///   The type of <see cref="System.Exception" /> that is being thrown.
    /// </typeparam>
    /// <param name="exception">
    ///   The <see cref="System.Exception" /> to throw.
    /// </param>
    [DebuggerHidden]
    public static void Fail<TException>( TException exception )
      where TException : System.Exception
    {
      ThrowGenericException<TException>( exception );
    }

    /// <summary>
    ///   Declares a fail condition by throwing an exception.
    /// </summary>
    /// <typeparam name="T">
    ///   The return type of the call.
    /// </typeparam>
    /// <returns>
    ///   A default value for the generic type passed in.
    /// </returns>
    /// <remarks>
    ///   This can be used in <c>return</c> statements for cleaner code flow.
    /// </remarks>
    [DebuggerHidden]
    public static T FailReturn<T>()
    {
      ThrowAssertionFailedException();
      return default;
    }

    /// <summary>
    ///   Declares a fail condition by throwing an exception.
    /// </summary>
    /// <typeparam name="T">
    ///   The return type of the call.
    /// </typeparam>
    /// <param name="reason">
    ///   The reason a fail condition was hit.
    /// </param>
    /// <returns>
    ///   A default value for the generic type passed in.
    /// </returns>
    /// <remarks>
    ///   This can be used in <c>return</c> statements for cleaner code flow.
    /// </remarks>
    [DebuggerHidden]
    public static T FailReturn<T>( string reason )
    {
      ThrowAssertionFailedException( reason );
      return default;
    }

    #endregion

    #region Warn Methods

    [DebuggerHidden]
    public static void Warn( string format, params object[] args )
    {
      var col = Console.ForegroundColor;
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine( format, args );
      Console.ForegroundColor = col;
    }

    [DebuggerHidden]
    public static void WarnIf( bool expression, string format, params object[] args )
    {
      if ( expression )
        Warn( format, args );
    }

    #endregion

    #region Helper Methods

    /// <summary>
    ///   Throws an empty <see cref="AssertionFailedException" />.
    /// </summary>
    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.NoInlining )]
    internal static void ThrowAssertionFailedException()
    {
      throw new AssertionFailedException();
    }

    /// <summary>
    ///   Throws an <see cref="AssertionFailedException" /> with a specified reason.
    /// </summary>
    /// <param name="reason">
    ///   The reason why the assertion failed.
    /// </param>
    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.NoInlining )]
    internal static void ThrowAssertionFailedException( string reason )
    {
      throw new AssertionFailedException( reason );
    }

    /// <summary>
    ///   Throws a generic exception.
    /// </summary>
    /// <typeparam name="TException">
    ///   The type of exception to throw.
    /// </typeparam>
    /// <param name="exception">
    ///   The exception to throw.
    /// </param>
    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.NoInlining )]
    internal static void ThrowGenericException<TException>( TException exception )
      where TException : System.Exception
    {
      throw exception;
    }

    #endregion

    #region Exceptions

    public class AssertionFailedException : Exception
    {

      #region Constants

      private const string GENERIC_MESSAGE = "Assertion Failed.";

      #endregion

      #region Constructor

      public AssertionFailedException()
        : base( GENERIC_MESSAGE )
      {
      }

      public AssertionFailedException( string message )
        : base( BuildAssertionFailedMessage( message ) )
      {
      }

      #endregion

      #region Private Methods

      private static string BuildAssertionFailedMessage( string message )
      {
        if ( string.IsNullOrWhiteSpace( message ) )
          return GENERIC_MESSAGE;

        return $"Assertion Failed: {message}";
      }

      #endregion

    }

    #endregion

  }

}
