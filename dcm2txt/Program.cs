﻿/*
 * Copyright (c) 2011  Anders Gustafsson, Cureos AB.
 * This file is part of mdcm.
 *
 * mdcm is free software: you can redistribute it and/or 
 * modify it under the terms of the GNU Lesser General Public 
 * License as published by the Free Software Foundation, either 
 * version 3 of the License, or (at your option) any later version.
 *
 * mdcm is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public 
 * License along with mdcm.  
 * If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Text;
using Dicom;
using Dicom.Data;
using Dicom.IO;

namespace dcm2txt
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0 || args.Length > 2 ||
                (args.Length == 2 && args[0].Equals(args[1], StringComparison.InvariantCultureIgnoreCase)))
            {
                Console.WriteLine("usage: dcm2txt dcmfile-in [txtfile-out]");
                return;
            }

            var dcmFileName = args[0];

            if (!File.Exists(dcmFileName))
            {
                Console.WriteLine("dcm2txt: Specified DICOM file '{0}' does not exist or cannot be accessed.",
                                  dcmFileName);
                return;
            }

            if (File.Exists("dicom.dic"))
                DcmDictionary.ImportDictionary("dicom.dic");
            else
                DcmDictionary.LoadInternalDictionary();

            if (File.Exists("private.dic"))
                DcmDictionary.ImportDictionary("private.dic");

            string dump;
            try
            {
                var dcmFile = new DicomFileFormat();
                if (dcmFile.Load(dcmFileName, DicomReadOptions.Default) == DicomReadStatus.Success)
                {
                    var sb = new StringBuilder();

                    if (dcmFile.FileMetaInfo != null)
                        dcmFile.FileMetaInfo.Dump(sb, String.Empty, DicomDumpOptions.None);

                    if (dcmFile.Dataset != null)
                    {
                        dcmFile.Dataset.Dump(sb, String.Empty, DicomDumpOptions.None);
                    }
                    else
                    {
                        Console.WriteLine("dcm2txt: Missing dataset in DICOM file '{0}'.", dcmFileName);
                        return;
                    }
                    dump = sb.ToString();
                }
                else
                {
                    Console.WriteLine("dcm2txt: '{0}' does not appear to be a DICOM file.", dcmFileName);
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("dcm2txt: Dumping DICOM file to text failed, reason: {0}.", e.Message);
                return;
            }

            try
            {
                if (args.Length == 2)
                {
                    var txtFileName = args[1];
                    File.WriteAllText(txtFileName, dump);
                }
                else
                {
                    Console.Write(dump);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("dcm2txt: Writing DICOM dump to file/console failed, reason: {0}.", e.Message);
            }
        }
    }
}
