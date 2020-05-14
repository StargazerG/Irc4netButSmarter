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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StargazerG.Irc4NetButSmarter
{
    public class IrcCommands : IrcConnection
    {
        protected int MaxModeChanges { get; set; } = 3;

#if LOG4NET
        public IrcCommands()
        {
            Logger.Main.Debug("IrcCommands created");
        }

        ~IrcCommands()
        {
            Logger.Main.Debug("IrcCommands destroyed");
        }
#endif

        public async Task SendMessage(SendType type, string destination, string message, Priority priority = Priority.Medium)
        {
            switch (type)
            {
                case SendType.Message:
                    await RfcPrivmsg(destination, message, priority);
                    break;
                case SendType.Action:
                    await RfcPrivmsg(destination, "\x1" + "ACTION " + message + "\x1", priority);
                    break;
                case SendType.Notice:
                    await RfcNotice(destination, message, priority);
                    break;
                case SendType.CtcpRequest:
                    await RfcPrivmsg(destination, "\x1" + message + "\x1", priority);
                    break;
                case SendType.CtcpReply:
                    await RfcNotice(destination, "\x1" + message + "\x1", priority);
                    break;
            }
        }

        public async Task SendReply(IrcMessageData data, string message, Priority priority = Priority.Medium)
        {
            switch (data.Type)
            {
                case ReceiveType.ChannelMessage:
                    await SendMessage(SendType.Message, data.Channel, message, priority);
                    break;
                case ReceiveType.QueryMessage:
                    await SendMessage(SendType.Message, data.Nick, message, priority);
                    break;
                case ReceiveType.QueryNotice:
                    await SendMessage(SendType.Notice, data.Nick, message, priority);
                    break;
            }
        }

        /// <summary>
        /// Give or take a user's privilege in a channel.
        /// </summary>
        /// <param name="modechg">The mode change (e.g. +o) to perform on the user.</param>
        /// <param name="channel">The channel in which to perform the privilege change.</param>
        /// <param name="nickname">The nickname of the user whose privilege is being changed.</param>
        /// <param name="priority">The priority with which the mode-setting message should be sent.</param>
        public async Task ChangeChannelPrivilege(string modechg, string channel, string nickname, Priority priority) => await WriteLine(Rfc2812.Mode(channel, modechg + " " + nickname), priority);

        /// <summary>
        /// Give or take a user's privilege in a channel.
        /// </summary>
        /// <param name="modechg">The mode change (e.g. +o) to perform on the user.</param>
        /// <param name="channel">The channel in which to perform the privilege change.</param>
        /// <param name="nickname">The nickname of the user whose privilege is being changed.</param>
        public async Task ChangeChannelPrivilege(string modechg, string channel, string nickname) => await WriteLine(Rfc2812.Mode(channel, modechg + " " + nickname));

        /// <summary>
        /// Give or take a privilege to/from multiple users in a channel.
        /// </summary>
        /// <param name="modechg">The mode change (e.g. +o) to perform on the users.</param>
        /// <param name="channel">The channel in which to give the users a privilege.</param>
        /// <param name="nickname">The nicknames of the users receiving the privilege.</param>
        public async Task ChangeChannelPrivilege(string modechg, string channel, string[] nicknames)
        {
            if (nicknames == null)
            {
                throw new ArgumentNullException(nameof(nicknames));
            }

            string[] modes = new string[nicknames.Length];
            for (int i = 0; i < nicknames.Length; i++)
            {
                modes[i] = modechg;
            }
            await Mode(channel, modes, nicknames);
        }

        public async Task Op(string channel, string nickname, Priority priority) => await ChangeChannelPrivilege("+o", channel, nickname, priority);

        public async Task Op(string channel, string[] nicknames) => await ChangeChannelPrivilege("+o", channel, nicknames);

        public async Task Op(string channel, string nickname) => await ChangeChannelPrivilege("+o", channel, nickname);

        public async Task Deop(string channel, string nickname, Priority priority) => await ChangeChannelPrivilege("-o", channel, nickname, priority);

        public async Task Deop(string channel, string[] nicknames) => await ChangeChannelPrivilege("-o", channel, nicknames);

        public async Task Deop(string channel, string nickname) => await ChangeChannelPrivilege("-o", channel, nickname);

        public async Task Voice(string channel, string nickname, Priority priority) => await ChangeChannelPrivilege("+v", channel, nickname, priority);

        public async Task Voice(string channel, string[] nicknames) => await ChangeChannelPrivilege("+v", channel, nicknames);

        public async Task Voice(string channel, string nickname) => await ChangeChannelPrivilege("+v", channel, nickname);

        public async Task Devoice(string channel, string nickname, Priority priority) => await ChangeChannelPrivilege("-v", channel, nickname, priority);

        public async Task Devoice(string channel, string[] nicknames) => await ChangeChannelPrivilege("-v", channel, nicknames);

        public async Task Devoice(string channel, string nickname) => await ChangeChannelPrivilege("-v", channel, nickname);

        /// <summary>
        /// Fetch a list of entries of a mask-format channel mode.
        /// </summary>
        /// <param name="modetype">The type of the mask-format mode (e.g. +b) to fetch.</param>
        /// <param name="channel">The channel whose mode to fetch.</param>
        public async Task ListChannelMasks(string modetype, string channel) => await WriteLine(Rfc2812.Mode(channel, modetype));

        /// <summary>
        /// Fetch a list of entries of a mask-format channel mode.
        /// </summary>
        /// <param name="modetype">The type of the mask-format mode (e.g. +b) to fetch.</param>
        /// <param name="channel">The channel whose mode to fetch.</param>
        /// <param name="priority">The priority with which the mode-setting message should be sent.</param>
        public async Task ListChannelMasks(string modetype, string channel, Priority priority) => await WriteLine(Rfc2812.Mode(channel, modetype), priority);

        /// <summary>
        /// Add or remove an entry to/from a mask-format channel mode.
        /// </summary>
        /// <param name="modetype">The type of the mask-format mode (e.g. +b) whose entries to modify.</param>
        /// <param name="channel">The channel whose mode to edit.</param>
        /// <param name="hostmask">The hostmask of the entry to add/remove.</param>
        /// <param name="priority">The priority with which the mode-setting message should be sent.</param>
        public async Task ModifyChannelMasks(string modetype, string channel, string hostmask, Priority priority) => await WriteLine(Rfc2812.Mode(channel, modetype + " " + hostmask), priority);

        /// <summary>
        /// Add or remove an entry to/from a mask-format channel mode.
        /// </summary>
        /// <param name="modetype">The type of the mask-format mode (e.g. +b) whose entries to modify.</param>
        /// <param name="channel">The channel whose mode to edit.</param>
        /// <param name="hostmask">The hostmask of the entry to add/remove.</param>
        public async Task ModifyChannelMasks(string modetype, string channel, string hostmask) => await WriteLine(Rfc2812.Mode(channel, modetype + " " + hostmask));

        /// <summary>
        /// Add or remove multiple entries to/from a mask-format channel mode.
        /// </summary>
        /// <param name="modetype">The type of the mask-format mode (e.g. +b) whose entries to modify.</param>
        /// <param name="channel">The channel whose mode to edit.</param>
        /// <param name="hostmasks">The hostmasks of the entries to add/remove.</param>
        public async Task ModifyChannelMasks(string modetype, string channel, string[] hostmasks)
        {
            if (hostmasks == null)
            {
                throw new ArgumentNullException(nameof(hostmasks));
            }

            string[] modes = new string[hostmasks.Length];
            for (int i = 0; i < hostmasks.Length; i++)
            {
                modes[i] = modetype;
            }
            await Mode(channel, modes, hostmasks);
        }

        public async Task Ban(string channel) => await ListChannelMasks("+b", channel);

        public async Task Ban(string channel, string hostmask, Priority priority) => await ModifyChannelMasks("+b", channel, hostmask, priority);

        public async Task Ban(string channel, string hostmask) => await ModifyChannelMasks("+b", channel, hostmask);

        public async Task Ban(string channel, string[] hostmasks) => await ModifyChannelMasks("+b", channel, hostmasks);

        public async Task Unban(string channel, string hostmask, Priority priority) => await ModifyChannelMasks("-b", channel, hostmask, priority);

        public async Task Unban(string channel, string hostmask) => await ModifyChannelMasks("-b", channel, hostmask);

        public async Task Unban(string channel, string[] hostmasks) => await ModifyChannelMasks("-b", channel, hostmasks);

        public virtual async Task BanException(string channel) => await ListChannelMasks("+e", channel);

        public virtual async Task BanException(string channel, string hostmask, Priority priority) => await ModifyChannelMasks("+e", channel, hostmask, priority);

        public virtual async Task BanException(string channel, string hostmask) => await ModifyChannelMasks("+e", channel, hostmask);

        public virtual async Task BanException(string channel, string[] hostmasks) => await ModifyChannelMasks("+e", channel, hostmasks);

        public virtual async Task UnBanException(string channel, string hostmask, Priority priority) => await ModifyChannelMasks("-e", channel, hostmask, priority);

        public virtual async Task UnBanException(string channel, string hostmask) => await ModifyChannelMasks("-e", channel, hostmask);

        public virtual async Task UnBanException(string channel, string[] hostmasks) => await ModifyChannelMasks("-e", channel, hostmasks);

        public virtual async Task InviteException(string channel) => await ListChannelMasks("+I", channel);

        public virtual async Task InviteException(string channel, string hostmask, Priority priority) => await ModifyChannelMasks("+I", channel, hostmask, priority);

        public virtual async Task InviteException(string channel, string hostmask) => await ModifyChannelMasks("+I", channel, hostmask);

        public virtual async Task InviteException(string channel, string[] hostmasks) => await ModifyChannelMasks("+I", channel, hostmasks);

        public virtual async Task UnInviteException(string channel, string hostmask, Priority priority) => await ModifyChannelMasks("-I", channel, hostmask, priority);

        public virtual async Task UnInviteException(string channel, string hostmask) => await ModifyChannelMasks("-I", channel, hostmask);

        public virtual async Task UnInviteException(string channel, string[] hostmasks) => await ModifyChannelMasks("-I", channel, hostmasks);

        // non-RFC commands

        public async Task Owner(string channel, string nickname, Priority priority) => await ChangeChannelPrivilege("+q", channel, nickname, priority);

        public async Task Owner(string channel, string[] nicknames) => await ChangeChannelPrivilege("+q", channel, nicknames);

        public async Task Owner(string channel, string nickname) => await ChangeChannelPrivilege("+q", channel, nickname);

        public async Task Deowner(string channel, string nickname, Priority priority) => await ChangeChannelPrivilege("-q", channel, nickname, priority);

        public async Task Deowner(string channel, string[] nicknames) => await ChangeChannelPrivilege("-q", channel, nicknames);

        public async Task Deowner(string channel, string nickname) => await ChangeChannelPrivilege("-q", channel, nickname);

        public async Task ChanAdmin(string channel, string nickname, Priority priority) => await ChangeChannelPrivilege("+a", channel, nickname, priority);

        public async Task ChanAdmin(string channel, string[] nicknames) => await ChangeChannelPrivilege("+a", channel, nicknames);

        public async Task ChanAdmin(string channel, string nickname) => await ChangeChannelPrivilege("+a", channel, nickname);

        public async Task DeChanAdmin(string channel, string nickname, Priority priority) => await ChangeChannelPrivilege("-a", channel, nickname, priority);

        public async Task DeChanAdmin(string channel, string[] nicknames) => await ChangeChannelPrivilege("-a", channel, nicknames);

        public async Task DeChanAdmin(string channel, string nickname) => await ChangeChannelPrivilege("-a", channel, nickname);

        public async Task Halfop(string channel, string nickname, Priority priority) => await ChangeChannelPrivilege("+h", channel, nickname, priority);

        public async Task Halfop(string channel, string[] nicknames) => await ChangeChannelPrivilege("+h", channel, nicknames);

        public async Task Halfop(string channel, string nickname) => await ChangeChannelPrivilege("+h", channel, nickname);

        public async Task Dehalfop(string channel, string nickname, Priority priority) => await ChangeChannelPrivilege("-h", channel, nickname, priority);

        public async Task Dehalfop(string channel, string[] nicknames) => await ChangeChannelPrivilege("-h", channel, nicknames);

        public async Task Dehalfop(string channel, string nickname) => await ChangeChannelPrivilege("-h", channel, nickname);

        public async Task Mode(string target, string[] newModes, string[] newModeParameters)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if (newModes == null)
            {
                throw new ArgumentNullException(nameof(newModes));
            }
            if (newModeParameters == null)
            {
                throw new ArgumentNullException(nameof(newModeParameters));
            }
            if (newModes.Length == 0)
            {
                throw new ArgumentException("newModes must not be empty.", nameof(newModes));
            }
            if (newModeParameters.Length == 0)
            {
                throw new ArgumentException("newModeParameters must not be empty.", nameof(newModeParameters));
            }
            if (newModes.Length != newModeParameters.Length)
            {
                throw new ArgumentException("newModes and newModeParameters must have the same size.", nameof(newModes));
            }

            int maxModeChanges = MaxModeChanges;
            for (int i = 0; i < newModes.Length; i += maxModeChanges)
            {
                var newModeChunks = new List<string>(maxModeChanges);
                var newModeParameterChunks = new List<string>(maxModeChanges);
                for (int j = 0; j < maxModeChanges; j++)
                {
                    if (i + j >= newModes.Length)
                    {
                        break;
                    }
                    newModeChunks.Add(newModes[i + j]);
                    newModeParameterChunks.Add(newModeParameters[i + j]);
                }
                await WriteLine(Rfc2812.Mode(target, newModeChunks.ToArray(), newModeParameterChunks.ToArray()));
            }
        }

        public async Task RfcPass(string password, Priority priority) => await WriteLine(Rfc2812.Pass(password), priority);

        public async Task RfcPass(string password) => await WriteLine(Rfc2812.Pass(password));

        public async Task RfcUser(string username, int usermode, string realname, Priority priority) => await WriteLine(Rfc2812.User(username, usermode, realname), priority);

        public async Task RfcUser(string username, int usermode, string realname) => await WriteLine(Rfc2812.User(username, usermode, realname));

        public async Task RfcOper(string name, string password, Priority priority) => await WriteLine(Rfc2812.Oper(name, password), priority);

        public async Task RfcOper(string name, string password) => await WriteLine(Rfc2812.Oper(name, password));

        public async Task RfcPrivmsg(string destination, string message, Priority priority) => await WriteLine(Rfc2812.Privmsg(destination, message), priority);

        public async Task RfcPrivmsg(string destination, string message) => await WriteLine(Rfc2812.Privmsg(destination, message));

        public async Task RfcNotice(string destination, string message, Priority priority) => await WriteLine(Rfc2812.Notice(destination, message), priority);

        public async Task RfcNotice(string destination, string message) => await WriteLine(Rfc2812.Notice(destination, message));

        public async Task RfcJoin(string channel, Priority priority) => await WriteLine(Rfc2812.Join(channel), priority);

        public async Task RfcJoin(string channel) => await WriteLine(Rfc2812.Join(channel));

        public async Task RfcJoin(string[] channels, Priority priority) => await WriteLine(Rfc2812.Join(channels), priority);

        public async Task RfcJoin(string[] channels) => await WriteLine(Rfc2812.Join(channels));

        public async Task RfcJoin(string channel, string key, Priority priority) => await WriteLine(Rfc2812.Join(channel, key), priority);

        public async Task RfcJoin(string channel, string key) => await WriteLine(Rfc2812.Join(channel, key));

        public async Task RfcJoin(string[] channels, string[] keys, Priority priority) => await WriteLine(Rfc2812.Join(channels, keys), priority);

        public async Task RfcJoin(string[] channels, string[] keys) => await WriteLine(Rfc2812.Join(channels, keys));

        public async Task RfcPart(string channel, Priority priority) => await WriteLine(Rfc2812.Part(channel), priority);

        public async Task RfcPart(string channel) => await WriteLine(Rfc2812.Part(channel));

        public async Task RfcPart(string[] channels, Priority priority) => await WriteLine(Rfc2812.Part(channels), priority);

        public async Task RfcPart(string[] channels) => await WriteLine(Rfc2812.Part(channels));

        public async Task RfcPart(string channel, string partmessage, Priority priority) => await WriteLine(Rfc2812.Part(channel, partmessage), priority);

        public async Task RfcPart(string channel, string partmessage) => await WriteLine(Rfc2812.Part(channel, partmessage));

        public async Task RfcPart(string[] channels, string partmessage, Priority priority) => await WriteLine(Rfc2812.Part(channels, partmessage), priority);

        public async Task RfcPart(string[] channels, string partmessage) => await WriteLine(Rfc2812.Part(channels, partmessage));

        public async Task RfcKick(string channel, string nickname, Priority priority) => await WriteLine(Rfc2812.Kick(channel, nickname), priority);

        public async Task RfcKick(string channel, string nickname) => await WriteLine(Rfc2812.Kick(channel, nickname));

        public async Task RfcKick(string[] channels, string nickname, Priority priority) => await WriteLine(Rfc2812.Kick(channels, nickname), priority);

        public async Task RfcKick(string[] channels, string nickname) => await WriteLine(Rfc2812.Kick(channels, nickname));

        public async Task RfcKick(string channel, string[] nicknames, Priority priority) => await WriteLine(Rfc2812.Kick(channel, nicknames), priority);

        public async Task RfcKick(string channel, string[] nicknames) => await WriteLine(Rfc2812.Kick(channel, nicknames));

        public async Task RfcKick(string[] channels, string[] nicknames, Priority priority) => await WriteLine(Rfc2812.Kick(channels, nicknames), priority);

        public async Task RfcKick(string[] channels, string[] nicknames) => await WriteLine(Rfc2812.Kick(channels, nicknames));

        public async Task RfcKick(string channel, string nickname, string comment, Priority priority) => await WriteLine(Rfc2812.Kick(channel, nickname, comment), priority);

        public async Task RfcKick(string channel, string nickname, string comment) => await WriteLine(Rfc2812.Kick(channel, nickname, comment));

        public async Task RfcKick(string[] channels, string nickname, string comment, Priority priority) => await WriteLine(Rfc2812.Kick(channels, nickname, comment), priority);

        public async Task RfcKick(string[] channels, string nickname, string comment) => await WriteLine(Rfc2812.Kick(channels, nickname, comment));

        public async Task RfcKick(string channel, string[] nicknames, string comment, Priority priority) => await WriteLine(Rfc2812.Kick(channel, nicknames, comment), priority);

        public async Task RfcKick(string channel, string[] nicknames, string comment) => await WriteLine(Rfc2812.Kick(channel, nicknames, comment));

        public async Task RfcKick(string[] channels, string[] nicknames, string comment, Priority priority) => await WriteLine(Rfc2812.Kick(channels, nicknames, comment), priority);

        public async Task RfcKick(string[] channels, string[] nicknames, string comment) => await WriteLine(Rfc2812.Kick(channels, nicknames, comment));

        public async Task RfcMotd(Priority priority) => await WriteLine(Rfc2812.Motd(), priority);

        public async Task RfcMotd() => await WriteLine(Rfc2812.Motd());

        public async Task RfcMotd(string target, Priority priority) => await WriteLine(Rfc2812.Motd(target), priority);

        public async Task RfcMotd(string target) => await WriteLine(Rfc2812.Motd(target));

        public async Task RfcLusers(Priority priority) => await WriteLine(Rfc2812.Lusers(), priority);

        public async Task RfcLusers() => await WriteLine(Rfc2812.Lusers());

        public async Task RfcLusers(string mask, Priority priority) => await WriteLine(Rfc2812.Lusers(mask), priority);

        public async Task RfcLusers(string mask) => await WriteLine(Rfc2812.Lusers(mask));

        public async Task RfcLusers(string mask, string target, Priority priority) => await WriteLine(Rfc2812.Lusers(mask, target), priority);

        public async Task RfcLusers(string mask, string target) => await WriteLine(Rfc2812.Lusers(mask, target));

        public async Task RfcVersion(Priority priority) => await WriteLine(Rfc2812.Version(), priority);

        public async Task RfcVersion() => await WriteLine(Rfc2812.Version());

        public async Task RfcVersion(string target, Priority priority) => await WriteLine(Rfc2812.Version(target), priority);

        public async Task RfcVersion(string target) => await WriteLine(Rfc2812.Version(target));

        public async Task RfcStats(Priority priority) => await WriteLine(Rfc2812.Stats(), priority);

        public async Task RfcStats() => await WriteLine(Rfc2812.Stats());

        public async Task RfcStats(string query, Priority priority) => await WriteLine(Rfc2812.Stats(query), priority);

        public async Task RfcStats(string query) => await WriteLine(Rfc2812.Stats(query));

        public async Task RfcStats(string query, string target, Priority priority) => await WriteLine(Rfc2812.Stats(query, target), priority);

        public async Task RfcStats(string query, string target) => await WriteLine(Rfc2812.Stats(query, target));

        public async Task RfcLinks() => await WriteLine(Rfc2812.Links());

        public async Task RfcLinks(string servermask, Priority priority) => await WriteLine(Rfc2812.Links(servermask), priority);

        public async Task RfcLinks(string servermask) => await WriteLine(Rfc2812.Links(servermask));

        public async Task RfcLinks(string remoteserver, string servermask, Priority priority) => await WriteLine(Rfc2812.Links(remoteserver, servermask), priority);

        public async Task RfcLinks(string remoteserver, string servermask) => await WriteLine(Rfc2812.Links(remoteserver, servermask));

        public async Task RfcTime(Priority priority) => await WriteLine(Rfc2812.Time(), priority);

        public async Task RfcTime() => await WriteLine(Rfc2812.Time());

        public async Task RfcTime(string target, Priority priority) => await WriteLine(Rfc2812.Time(target), priority);

        public async Task RfcTime(string target) => await WriteLine(Rfc2812.Time(target));

        public async Task RfcConnect(string targetserver, string port, Priority priority) => await WriteLine(Rfc2812.Connect(targetserver, port), priority);

        public async Task RfcConnect(string targetserver, string port) => await WriteLine(Rfc2812.Connect(targetserver, port));

        public async Task RfcConnect(string targetserver, string port, string remoteserver, Priority priority) => await WriteLine(Rfc2812.Connect(targetserver, port, remoteserver), priority);

        public async Task RfcConnect(string targetserver, string port, string remoteserver) => await WriteLine(Rfc2812.Connect(targetserver, port, remoteserver));

        public async Task RfcTrace(Priority priority) => await WriteLine(Rfc2812.Trace(), priority);

        public async Task RfcTrace() => await WriteLine(Rfc2812.Trace());

        public async Task RfcTrace(string target, Priority priority) => await WriteLine(Rfc2812.Trace(target), priority);

        public async Task RfcTrace(string target) => await WriteLine(Rfc2812.Trace(target));

        public async Task RfcAdmin(Priority priority) => await WriteLine(Rfc2812.Admin(), priority);

        public async Task RfcAdmin() => await WriteLine(Rfc2812.Admin());

        public async Task RfcAdmin(string target, Priority priority) => await WriteLine(Rfc2812.Admin(target), priority);

        public async Task RfcAdmin(string target) => await WriteLine(Rfc2812.Admin(target));

        public async Task RfcInfo(Priority priority) => await WriteLine(Rfc2812.Info(), priority);

        public async Task RfcInfo() => await WriteLine(Rfc2812.Info());

        public async Task RfcInfo(string target, Priority priority) => await WriteLine(Rfc2812.Info(target), priority);

        public async Task RfcInfo(string target) => await WriteLine(Rfc2812.Info(target));

        public async Task RfcServlist(Priority priority) => await WriteLine(Rfc2812.Servlist(), priority);

        public async Task RfcServlist() => await WriteLine(Rfc2812.Servlist());

        public async Task RfcServlist(string mask, Priority priority) => await WriteLine(Rfc2812.Servlist(mask), priority);

        public async Task RfcServlist(string mask) => await WriteLine(Rfc2812.Servlist(mask));

        public async Task RfcServlist(string mask, string type, Priority priority) => await WriteLine(Rfc2812.Servlist(mask, type), priority);

        public async Task RfcServlist(string mask, string type) => await WriteLine(Rfc2812.Servlist(mask, type));

        public async Task RfcSquery(string servicename, string servicetext, Priority priority) => await WriteLine(Rfc2812.Squery(servicename, servicetext), priority);

        public async Task RfcSquery(string servicename, string servicetext) => await WriteLine(Rfc2812.Squery(servicename, servicetext));

        public async Task RfcList(string channel, Priority priority) => await WriteLine(Rfc2812.List(channel), priority);

        public async Task RfcList(string channel) => await WriteLine(Rfc2812.List(channel));

        public async Task RfcList(string[] channels, Priority priority) => await WriteLine(Rfc2812.List(channels), priority);

        public async Task RfcList(string[] channels) => await WriteLine(Rfc2812.List(channels));

        public async Task RfcList(string channel, string target, Priority priority) => await WriteLine(Rfc2812.List(channel, target), priority);

        public async Task RfcList(string channel, string target) => await WriteLine(Rfc2812.List(channel, target));

        public async Task RfcList(string[] channels, string target, Priority priority) => await WriteLine(Rfc2812.List(channels, target), priority);

        public async Task RfcList(string[] channels, string target) => await WriteLine(Rfc2812.List(channels, target));

        public async Task RfcNames(string channel, Priority priority) => await WriteLine(Rfc2812.Names(channel), priority);

        public async Task RfcNames(string channel) => await WriteLine(Rfc2812.Names(channel));

        public async Task RfcNames(string[] channels, Priority priority) => await WriteLine(Rfc2812.Names(channels), priority);

        public async Task RfcNames(string[] channels) => await WriteLine(Rfc2812.Names(channels));

        public async Task RfcNames(string channel, string target, Priority priority) => await WriteLine(Rfc2812.Names(channel, target), priority);

        public async Task RfcNames(string channel, string target) => await WriteLine(Rfc2812.Names(channel, target));

        public async Task RfcNames(string[] channels, string target, Priority priority) => await WriteLine(Rfc2812.Names(channels, target), priority);

        public async Task RfcNames(string[] channels, string target) => await WriteLine(Rfc2812.Names(channels, target));

        public async Task RfcTopic(string channel, Priority priority) => await WriteLine(Rfc2812.Topic(channel), priority);

        public async Task RfcTopic(string channel) => await WriteLine(Rfc2812.Topic(channel));

        public async Task RfcTopic(string channel, string newtopic, Priority priority) => await WriteLine(Rfc2812.Topic(channel, newtopic), priority);

        public async Task RfcTopic(string channel, string newtopic) => await WriteLine(Rfc2812.Topic(channel, newtopic));

        public async Task RfcMode(string target, Priority priority) => await WriteLine(Rfc2812.Mode(target), priority);

        public async Task RfcMode(string target) => await WriteLine(Rfc2812.Mode(target));

        public async Task RfcMode(string target, string newmode, Priority priority) => await WriteLine(Rfc2812.Mode(target, newmode), priority);

        public async Task RfcMode(string target, string newmode) => await WriteLine(Rfc2812.Mode(target, newmode));

        public async Task RfcService(string nickname, string distribution, string info, Priority priority) => await WriteLine(Rfc2812.Service(nickname, distribution, info), priority);

        public async Task RfcService(string nickname, string distribution, string info) => await WriteLine(Rfc2812.Service(nickname, distribution, info));

        public async Task RfcInvite(string nickname, string channel, Priority priority) => await WriteLine(Rfc2812.Invite(nickname, channel), priority);

        public async Task RfcInvite(string nickname, string channel) => await WriteLine(Rfc2812.Invite(nickname, channel));

        public async Task RfcNick(string newnickname, Priority priority) => await WriteLine(Rfc2812.Nick(newnickname), priority);

        public async Task RfcNick(string newnickname) => await WriteLine(Rfc2812.Nick(newnickname));

        public async Task RfcWho(Priority priority) => await WriteLine(Rfc2812.Who(), priority);

        public async Task RfcWho() => await WriteLine(Rfc2812.Who());

        public async Task RfcWho(string mask, Priority priority) => await WriteLine(Rfc2812.Who(mask), priority);

        public async Task RfcWho(string mask) => await WriteLine(Rfc2812.Who(mask));

        public async Task RfcWho(string mask, bool ircop, Priority priority) => await WriteLine(Rfc2812.Who(mask, ircop), priority);

        public async Task RfcWho(string mask, bool ircop) => await WriteLine(Rfc2812.Who(mask, ircop));

        public async Task RfcWhois(string mask, Priority priority) => await WriteLine(Rfc2812.Whois(mask), priority);

        public async Task RfcWhois(string mask) => await WriteLine(Rfc2812.Whois(mask));

        public async Task RfcWhois(string[] masks, Priority priority) => await WriteLine(Rfc2812.Whois(masks), priority);

        public async Task RfcWhois(string[] masks) => await WriteLine(Rfc2812.Whois(masks));

        public async Task RfcWhois(string target, string mask, Priority priority) => await WriteLine(Rfc2812.Whois(target, mask), priority);

        public async Task RfcWhois(string target, string mask) => await WriteLine(Rfc2812.Whois(target, mask));

        public async Task RfcWhois(string target, string[] masks, Priority priority) => await WriteLine(Rfc2812.Whois(target, masks), priority);

        public async Task RfcWhois(string target, string[] masks) => await WriteLine(Rfc2812.Whois(target, masks));

        public async Task RfcWhowas(string nickname, Priority priority) => await WriteLine(Rfc2812.Whowas(nickname), priority);

        public async Task RfcWhowas(string nickname) => await WriteLine(Rfc2812.Whowas(nickname));

        public async Task RfcWhowas(string[] nicknames, Priority priority) => await WriteLine(Rfc2812.Whowas(nicknames), priority);

        public async Task RfcWhowas(string[] nicknames) => await WriteLine(Rfc2812.Whowas(nicknames));

        public async Task RfcWhowas(string nickname, string count, Priority priority) => await WriteLine(Rfc2812.Whowas(nickname, count), priority);

        public async Task RfcWhowas(string nickname, string count) => await WriteLine(Rfc2812.Whowas(nickname, count));

        public async Task RfcWhowas(string[] nicknames, string count, Priority priority) => await WriteLine(Rfc2812.Whowas(nicknames, count), priority);

        public async Task RfcWhowas(string[] nicknames, string count) => await WriteLine(Rfc2812.Whowas(nicknames, count));

        public async Task RfcWhowas(string nickname, string count, string target, Priority priority) => await WriteLine(Rfc2812.Whowas(nickname, count, target), priority);

        public async Task RfcWhowas(string nickname, string count, string target) => await WriteLine(Rfc2812.Whowas(nickname, count, target));

        public async Task RfcWhowas(string[] nicknames, string count, string target, Priority priority) => await WriteLine(Rfc2812.Whowas(nicknames, count, target), priority);

        public async Task RfcWhowas(string[] nicknames, string count, string target) => await WriteLine(Rfc2812.Whowas(nicknames, count, target));

        public async Task RfcKill(string nickname, string comment, Priority priority) => await WriteLine(Rfc2812.Kill(nickname, comment), priority);

        public async Task RfcKill(string nickname, string comment) => await WriteLine(Rfc2812.Kill(nickname, comment));

        public async Task RfcPing(string server, Priority priority) => await WriteLine(Rfc2812.Ping(server), priority);

        public async Task RfcPing(string server) => await WriteLine(Rfc2812.Ping(server));

        public async Task RfcPing(string server, string server2, Priority priority) => await WriteLine(Rfc2812.Ping(server, server2), priority);

        public async Task RfcPing(string server, string server2) => await WriteLine(Rfc2812.Ping(server, server2));

        public async Task RfcPong(string server, Priority priority) => await WriteLine(Rfc2812.Pong(server), priority);

        public async Task RfcPong(string server) => await WriteLine(Rfc2812.Pong(server));

        public async Task RfcPong(string server, string server2, Priority priority) => await WriteLine(Rfc2812.Pong(server, server2), priority);

        public async Task RfcPong(string server, string server2) => await WriteLine(Rfc2812.Pong(server, server2));

        public async Task RfcAway(Priority priority) => await WriteLine(Rfc2812.Away(), priority);

        public async Task RfcAway() => await WriteLine(Rfc2812.Away());

        public async Task RfcAway(string awaytext, Priority priority) => await WriteLine(Rfc2812.Away(awaytext), priority);

        public async Task RfcAway(string awaytext) => await WriteLine(Rfc2812.Away(awaytext));

        public async Task RfcRehash() => await WriteLine(Rfc2812.Rehash());

        public async Task RfcDie() => await WriteLine(Rfc2812.Die());

        public async Task RfcRestart() => await WriteLine(Rfc2812.Restart());

        public async Task RfcSummon(string user, Priority priority) => await WriteLine(Rfc2812.Summon(user), priority);

        public async Task RfcSummon(string user) => await WriteLine(Rfc2812.Summon(user));

        public async Task RfcSummon(string user, string target, Priority priority) => await WriteLine(Rfc2812.Summon(user, target), priority);

        public async Task RfcSummon(string user, string target) => await WriteLine(Rfc2812.Summon(user, target));

        public async Task RfcSummon(string user, string target, string channel, Priority priority) => await WriteLine(Rfc2812.Summon(user, target, channel), priority);

        public async Task RfcSummon(string user, string target, string channel) => await WriteLine(Rfc2812.Summon(user, target, channel));

        public async Task RfcUsers(Priority priority) => await WriteLine(Rfc2812.Users(), priority);

        public async Task RfcUsers() => await WriteLine(Rfc2812.Users());

        public async Task RfcUsers(string target, Priority priority) => await WriteLine(Rfc2812.Users(target), priority);

        public async Task RfcUsers(string target) => await WriteLine(Rfc2812.Users(target));

        public async Task RfcWallops(string wallopstext, Priority priority) => await WriteLine(Rfc2812.Wallops(wallopstext), priority);

        public async Task RfcWallops(string wallopstext) => await WriteLine(Rfc2812.Wallops(wallopstext));

        public async Task RfcUserhost(string nickname, Priority priority) => await WriteLine(Rfc2812.Userhost(nickname), priority);

        public async Task RfcUserhost(string nickname) => await WriteLine(Rfc2812.Userhost(nickname));

        public async Task RfcUserhost(string[] nicknames, Priority priority) => await WriteLine(Rfc2812.Userhost(nicknames), priority);

        public async Task RfcUserhost(string[] nicknames) => await WriteLine(Rfc2812.Userhost(nicknames));

        public async Task RfcIson(string nickname, Priority priority) => await WriteLine(Rfc2812.Ison(nickname), priority);

        public async Task RfcIson(string nickname) => await WriteLine(Rfc2812.Ison(nickname));

        public async Task RfcIson(string[] nicknames, Priority priority) => await WriteLine(Rfc2812.Ison(nicknames), priority);

        public async Task RfcIson(string[] nicknames) => await WriteLine(Rfc2812.Ison(nicknames));

        public async Task RfcQuit(Priority priority) => await WriteLine(Rfc2812.Quit(), priority);

        public async Task RfcQuit() => await WriteLine(Rfc2812.Quit());

        public async Task RfcQuit(string quitmessage, Priority priority) => await WriteLine(Rfc2812.Quit(quitmessage), priority);

        public async Task RfcQuit(string quitmessage) => await WriteLine(Rfc2812.Quit(quitmessage));

        public async Task RfcSquit(string server, string comment, Priority priority) => await WriteLine(Rfc2812.Squit(server, comment), priority);

        public async Task RfcSquit(string server, string comment) => await WriteLine(Rfc2812.Squit(server, comment));
    }
}
