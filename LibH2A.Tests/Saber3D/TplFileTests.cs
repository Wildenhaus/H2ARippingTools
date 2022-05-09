using System;
using LibH2A.Saber3D.Templates;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibH2A.Tests
{

  [TestClass]
  public class TplFileTests
  {

    [TestMethod]
    public void TplFile_Reads_TPL_With_Multiple_Names_In_Pak_Metadata()
    {
      //== Arrange ==============================
      var file = TestHelper.GetFile( "int_foliage_alder_02__act_tree.tpl" );

      //== Act ==================================
      var tplFile = TplFile.Open( file );

      //== Assert ===============================

      //== Cleanup ==============================
      tplFile.Dispose();
    }

    [TestMethod]
    public void TplFile_Reads_TPL_File_Actor()
    {
      //== Arrange ==============================
      var file = TestHelper.GetFile( "banshee__h.tpl" );

      //== Act ==================================
      var tplFile = TplFile.Open( file );

      //== Assert ===============================

      //== Cleanup ==============================
      tplFile.Dispose();
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
          var tplFile = TplFile.Open( file );

          //== Assert ===============================

          //== Cleanup ==============================
          tplFile.Dispose();
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