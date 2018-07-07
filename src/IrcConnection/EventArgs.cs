/*
 * $Id$
 * $URL$
 * $Rev$
 * $Author$
 * $Date$
 *
 * SmartIrc4net - the IRC library for .NET/C# <http://smartirc4net.sf.net>
 *
 * Copyright (c) 2003-2005 Mirco Bauer <meebey@meebey.net> <http://www.meebey.net>
 * 
 * Full LGPL License: <http://www.gnu.org/licenses/lgpl.txt>
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;

namespace Meebey.SmartIrc4net
{
    public class ReadLineEventArgs : EventArgs
    {
        public string Line { get; }

        internal ReadLineEventArgs(string line) => Line = line;
    }

    /// <summary>
    ///
    /// </summary>
    public class WriteLineEventArgs : EventArgs
    {
        public string Line { get; }

        internal WriteLineEventArgs(string line) => Line = line;
    }

    /// <summary>
    /// 
    /// </summary>
    public class AutoConnectErrorEventArgs : EventArgs
    {
        public Exception Exception { get; }
        public string Address { get; }
        public int Port { get; }

        internal AutoConnectErrorEventArgs(string address, int port, Exception ex)
        {
            Address = address;
            Port = port;
            Exception = ex;
        }
    }
}
