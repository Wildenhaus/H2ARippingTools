using System;
using System.IO;
using LibH2A.Saber3D;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibH2A.Tests
{

  [TestClass]
  public class TplFileTests
  {

    [TestMethod]
    public void TplFile_Reads_TPL_File_With_Script()
    {
      //== Arrange ==============================
      var file = TestHelper.GetFile( "ss_prop__h.tpl" );

      //== Act ==================================
      var tplFile = S3D_Template.Open( File.OpenRead( file ) );

      //== Assert ===============================

    }

    [TestMethod]
    public void TplFile_Reads_All_TPL_Files()
    {
      //== Arrange ==============================
      var files = TestHelper.GetAllFilesWithExtension( "*.tpl" );

      foreach ( var file in files )
      {
        try
        {
          //== Act ==================================
          var tplFile = S3D_Template.Open( File.OpenRead( file ) );

          //== Assert ===============================

        }
        catch ( Exception ex )
        {
          var message = $"Failed to read {file}: {ex.Message}";
          throw new Exception( message );
        }
      }
    }

  }

}