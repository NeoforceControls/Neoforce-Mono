////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: ArchiveManager.cs                            //
//                                                            //
//      Version: 0.7                                          //
//                                                            //
//         Date: 11/09/2010                                   //
//                                                            //
//       Author: Tom Shane                                    //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//  Copyright (c) by Tom Shane                                //
//                                                            //
////////////////////////////////////////////////////////////////


#region //// Using /////////////

////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using TomShane.Neoforce.External.Zip;
using System.Globalization;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

  #region //// Classes ///////////
  
  ////////////////////////////////////////////////////////////////////////////  
  /// <include file='Documents/ArchiveManager.xml' path='ArchiveManager/Class[@name="ArchiveManager"]/*' />          
  public class ArchiveManager : ContentManager
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////
    private string archivePath = null;
    private ZipFile archive = null;
    private bool useArchive = false;   
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////
    /// <include file='Documents/ArchiveManager.xml' path='ArchiveManager/Member[@name="ArchivePath"]/*' />          
    public virtual string ArchivePath
    {
      get { return archivePath; }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public bool UseArchive
    {
      get { return useArchive; }
      set { useArchive = value; }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    #endregion

    #region //// Constructors //////

    ////////////////////////////////////////////////////////////////////////////
    /// <include file='Documents/ArchiveManager.xml' path='ArchiveManager/Member[@name="ArchiveManager"]/*' />              
    public ArchiveManager(IServiceProvider serviceProvider) : this(serviceProvider, null) { }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    /// <include file='Documents/ArchiveManager.xml' path='ArchiveManager/Member[@name="ArchiveManager1"]/*' />                  
    public ArchiveManager(IServiceProvider serviceProvider, string archive): base(serviceProvider)
    {
      if (archive != null)
      {
        this.archive = ZipFile.Read(archive);
        archivePath = archive;
        useArchive = true;
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Methods ///////////
    
    ////////////////////////////////////////////////////////////////////////////        
    /// <include file='Documents/ArchiveManager.xml' path='ArchiveManager/Member[@name="OpenStream"]/*' />
    protected override Stream OpenStream(string assetName)
    {
      if (useArchive && archive != null)
      {
        assetName = assetName.Replace("\\", "/");
        if (assetName.StartsWith("/")) assetName = assetName.Remove(0, 1);

        string fullAssetName = (assetName + ".xnb").ToLower();

        foreach (ZipEntry entry in archive)
        {
          ZipDirEntry ze = new ZipDirEntry(entry);
          
          string entryName = entry.FileName.ToLower();

          if (entryName == fullAssetName)
          {
            return entry.GetStream();
          }
        }
        throw new Exception("Cannot find asset \"" + assetName + "\" in the archive.");
      }
      else
      {
        return base.OpenStream(assetName);
      }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////    
    /// <include file='Documents/ArchiveManager.xml' path='ArchiveManager/Member[@name="GetAssetNames"]/*' />
    public string[] GetAssetNames()
    {
      if (useArchive && archive != null)
      {
        List<string> filenames = new List<string>();

        foreach (ZipEntry entry in archive)
        {
          string name = entry.FileName;
          if (name.EndsWith(".xnb"))
          {
            name = name.Remove(name.Length - 4, 4);
            filenames.Add(name);
          }
        }
        return filenames.ToArray();
      }
      else
      {
        return null;
      }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////    
    /// <include file='Documents/ArchiveManager.xml' path='ArchiveManager/Member[@name="GetAssetNames1"]/*' />        
    public string[] GetAssetNames(string path)
    {
      if (useArchive && archive != null)
      {
        if (path != null && path != "" && path != "\\" && path != "/")
        {
          List<string> filenames = new List<string>();

          foreach (ZipEntry entry in archive)
          {            
            string name = entry.FileName;
            if (name.EndsWith(".xnb"))
            {
              name = name.Remove(name.Length - 4, 4);
            }

            string[] parts = name.Split('/');
            string dir = "";
            for (int i = 0; i < parts.Length - 1; i++)
            {
              dir += parts[i] + '/';
            }

            path = path.Replace("\\", "/");
            if (path.StartsWith("/")) path = path.Remove(0, 1);
            if (!path.EndsWith("/")) path += '/';

            if (dir.ToLower() == path.ToLower() && !name.EndsWith("/"))
            {
              filenames.Add(name);
            }
          }
          return filenames.ToArray();
        }
        else
        {
          return GetAssetNames();
        }
      }
      else
      {
        return null;
      }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////    
    /// <include file='Documents/ArchiveManager.xml' path='ArchiveManager/Member[@name="GetFileStream"]/*' />
    public Stream GetFileStream(string filename)
    {
      if (useArchive && archive != null)
      {
        filename = filename.Replace("\\", "/").ToLower();
        if (filename.StartsWith("/")) filename = filename.Remove(0, 1);

        foreach (ZipEntry entry in archive)
        {
          string entryName = entry.FileName.ToLower();

          if (entryName.Equals(filename))
            return entry.GetStream();
        }

        throw new Exception("Cannot find file \"" + filename + "\" in the archive.");
      }
      else
      {
        return null;
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public string[] GetDirectories(string path)
    {
      if (useArchive && archive != null)
      {
        if (path != null && path != "" && path != "\\" && path != "/")
        {
          List<string> dirs = new List<string>();

          path = path.Replace("\\", "/");
          if (path.StartsWith("/")) path = path.Remove(0, 1);
          if (!path.EndsWith("/")) path += '/';

          foreach (ZipEntry entry in archive)
          {
            string name = entry.FileName;                     
            if (name.ToLower().StartsWith(path.ToLower()))
            {
              int i = name.IndexOf("/", path.Length);
              string item = name.Substring(path.Length, i - path.Length) + "\\";
              if (!dirs.Contains(item))
              {
                dirs.Add(item);
              }  
            }            
          }
          return dirs.ToArray();
        }
        else
        {
          return GetAssetNames();
        }
      }
      else if (Directory.Exists(path))
      {
        string[] dirs = Directory.GetDirectories(path);
        
        for (int i = 0; i < dirs.Length; i++)
        {
          string[] parts = dirs[i].Split('\\');
          dirs[i] = parts[parts.Length - 1] + '\\';
        }
        
        return dirs;
      }
      else return null;
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

  }
  ////////////////////////////////////////////////////////////////////////////

  #endregion  
  
}
