using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi.SavedGame;
namespace Assets.Scripts
{
    class MySavedGameMetadata : ISavedGameMetadata
    {
        private bool _IsOpen;

        /// <summary>
        /// Returns true if this metadata can be used for operations related to raw file data (i.e.
        /// the binary data contained in the underlying file). Metadata returned by Open operations
        /// will be "Open". After an update to the file is committed or the metadata is used to resolve
        /// a conflict, the corresponding Metadata is closed, and IsOpen will return false.
        ///
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get
            {
                return _IsOpen;
            }
        }
        private String _FileName;
        /// <summary>
        /// Returns the filename for this saved game. A saved game filename will only consist of
        /// non-URL reserved characters (i.e. a-z, A-Z, 0-9, or the symbols "-", ".", "_", or "~")
        /// and will between 1 and 100 characters in length (inclusive).
        /// </summary>
        /// <value>The filename.</value>
        public string Filename
        {
            get
            {
                return _FileName;
            }
        }

        /// <summary>
        /// Returns a human-readable description of what the saved game contains. This may be null.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get;
        }

        /// <summary>
        /// A URL corresponding to the PNG-encoded image corresponding to this saved game. null if
        /// the saved game does not have a cover image.
        /// </summary>
        /// <value>The cover image URL.</value>
        public string CoverImageURL
        {
            get;
        }

        /// <summary>
        /// Returns the total time played by the player for this saved game. This value is
        /// developer-specified and may be tracked in any way that is appropriate to the game. Note
        /// that this value is specific to this specific saved game (unless the developer intentionally
        /// sets the same value on all saved games). If the value was not set, this will be equal to
        /// <code>TimeSpan.FromMilliseconds(0)</code>
        /// </summary>
        /// <value>The total time played.</value>
        public TimeSpan TotalTimePlayed
        {
            get;
        }

        /// <summary>
        /// A timestamp corresponding to the last modification to the underlying saved game. If the
        /// saved game is newly created, this value will correspond to the time the first Open
        /// occurred. Otherwise, this corresponds to time the last successful write occurred (either by
        /// CommitUpdate or Resolve methods).
        /// </summary>
        /// <value>The last modified timestamp.</value>
        public DateTime LastModifiedTimestamp
        {
            get;
        }
    }
}
