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

using System.Threading.Tasks;

namespace StargazerG.Irc4NetButSmarter
{
    public delegate Task IrcEventHandler(object sender, IrcEventArgs e);
    public delegate Task CtcpEventHandler(object sender, CtcpEventArgs e);
    public delegate Task ActionEventHandler(object sender, ActionEventArgs e);
    public delegate Task ErrorEventHandler(object sender, ErrorEventArgs e);
    public delegate Task PingEventHandler(object sender, PingEventArgs e);
    public delegate Task KickEventHandler(object sender, KickEventArgs e);
    public delegate Task JoinEventHandler(object sender, JoinEventArgs e);
    public delegate Task NamesEventHandler(object sender, NamesEventArgs e);
    public delegate Task ListEventHandler(object sender, ListEventArgs e);
    public delegate Task PartEventHandler(object sender, PartEventArgs e);
    public delegate Task InviteEventHandler(object sender, InviteEventArgs e);
    public delegate Task OwnerEventHandler(object sender, OwnerEventArgs e);
    public delegate Task DeownerEventHandler(object sender, DeownerEventArgs e);
    public delegate Task ChannelAdminEventHandler(object sender, ChannelAdminEventArgs e);
    public delegate Task DeChannelAdminEventHandler(object sender, DeChannelAdminEventArgs e);
    public delegate Task OpEventHandler(object sender, OpEventArgs e);
    public delegate Task DeopEventHandler(object sender, DeopEventArgs e);
    public delegate Task HalfopEventHandler(object sender, HalfopEventArgs e);
    public delegate Task DehalfopEventHandler(object sender, DehalfopEventArgs e);
    public delegate Task VoiceEventHandler(object sender, VoiceEventArgs e);
    public delegate Task DevoiceEventHandler(object sender, DevoiceEventArgs e);
    public delegate Task BanEventHandler(object sender, BanEventArgs e);
    public delegate Task UnbanEventHandler(object sender, UnbanEventArgs e);
    public delegate Task TopicEventHandler(object sender, TopicEventArgs e);
    public delegate Task TopicChangeEventHandler(object sender, TopicChangeEventArgs e);
    public delegate Task NickChangeEventHandler(object sender, NickChangeEventArgs e);
    public delegate Task QuitEventHandler(object sender, QuitEventArgs e);
    public delegate Task AwayEventHandler(object sender, AwayEventArgs e);
    public delegate Task WhoEventHandler(object sender, WhoEventArgs e);
    public delegate Task MotdEventHandler(object sender, MotdEventArgs e);
    public delegate Task PongEventHandler(object sender, PongEventArgs e);
    public delegate Task BounceEventHandler(object sender, BounceEventArgs e);
}
