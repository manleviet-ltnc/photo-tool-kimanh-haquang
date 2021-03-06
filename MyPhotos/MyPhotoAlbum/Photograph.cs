﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace Manning.MyPhotoAlbum
{
    /// <summary>
    /// The photograph class represents a photographic
    /// image stored in the file system
    /// </summary>
    public class Photograph : IDisposable, IFormattable, IEditableObject
    {
        private bool _editing = false;
        internal bool Editing
        {
            get { return _editing; }
            private set { _editing = value; }
        }

        private string _savedCaption;
        private string _savedPhotographer;
        private DateTime _savedDateTaken;
        private string _savedNotes;

        public event EventHandler Modified;

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
        }

        private Bitmap _bitmap;
        public Bitmap Image
        {
            get
            {
                if (_bitmap == null)
                    _bitmap = new Bitmap(_fileName);
                return _bitmap;
            }
        }

        private string _caption = "";
        public string Caption
        {
            get { return _caption; }
            set
            {
                if (_caption != value)
                {
                    _caption = value;
                    _hasChanged = true;
                }
            }
        }

        private string _photographer = "";
        public string Photographer
        {
            get { return _photographer; }
            set
            {
                if (_photographer != value)
                {
                    _photographer = value;
                    _hasChanged = true;
                }
            }
        }

        private DateTime _dateTaken = DateTime.Now;
        public DateTime DateTaken
        {
            get { return _dateTaken; }
            set
            {
                if (_dateTaken != value)
                {
                    _dateTaken = value;
                    _hasChanged = true;
                }
            }
        }

        private string _notes = "";
        public string Notes
        {
            get { return _notes; }
            set
            {
                if (_notes != value)
                {
                    _notes = value;
                    _hasChanged = true;
                }
            }
        }

        private bool _hasChanged = true;
        public bool HasChanged
        {
            get { return _hasChanged; }
            set { _hasChanged = value; }
        }

        public Photograph(string fileName)
        {
            _fileName = fileName;
            _bitmap = null;
            _caption = System.IO.Path.GetFileNameWithoutExtension(fileName);
        }

        public override bool Equals(object obj)
        {
            if (obj is Photograph)
            {
                Photograph p = (Photograph)obj;
                return string.Equals(FileName, p.FileName, StringComparison.InvariantCultureIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return FileName.ToLowerInvariant().GetHashCode();
        }

        public override string ToString()
        {
            return FileName;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "f";

            char first = format.ToLower()[0];
            if (format.Length == 1)
            {
                switch (first)
                {
                    case 'c': return Caption;
                    case 'd': return DateTaken.ToShortDateString();
                    case 'f': return FileName;
                }
            }
            else if (first == 'd')
                return DateTaken.ToString(format.Substring(1), formatProvider);

            throw new FormatException();
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(IFormatProvider fp)
        {
            return ToString(null, fp);
        }

        public void ReleaseImage()
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
        }

        public void Dispose()
        {
            ReleaseImage();
        }

        protected virtual void OnModified(EventArgs e)
        {
            if (Modified != null)
                Modified(this, e);
        }

        public void BeginEdit()
        {
            if (!Editing)
            {
                _savedCaption = Caption;
                _savedPhotographer = Photographer;
                _savedDateTaken = DateTaken;
                _savedNotes = Notes;
                Editing = true;
            }
        }

        public void CancelEdit()
        {
            if (Editing)
            {
                Caption = _savedCaption;
                Photographer = _savedPhotographer;
                DateTaken = _savedDateTaken;
                Notes = _savedNotes;
                Editing = false;
                HasChanged = false;
                OnModified(EventArgs.Empty);
            }
        }

        public void EndEdit()
        {
            if (Editing)
            {
                Editing = false;
                OnModified(EventArgs.Empty);
            }
        }
    }
}