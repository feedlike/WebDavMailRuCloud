﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MailRuCloudApi;

namespace YaR.WebDavMailRu.CloudStore
{
    public static class TwoFaHandlers
    {
        static TwoFaHandlers()
        {
            _handlerTypes = GetHandlers().ToList();
        }

        private static List<Type> _handlerTypes;


        public static ITwoFaHandler Get(string name)
        {
            var type = _handlerTypes.FirstOrDefault(t => t.Name == name);
            if (null == type) return null;

            var inst = (ITwoFaHandler)Activator.CreateInstance(type);
            return inst;
        }

        private static IEnumerable<Type> GetHandlers()
        {
            foreach (var file in Directory.EnumerateFiles(Path.GetDirectoryName(typeof(TwoFaHandlers).Assembly.Location), "MailRuCloudApi.TwoFA*.dll", SearchOption.TopDirectoryOnly))
            {
                Assembly assembly = Assembly.LoadFile(file);
                foreach (var type in assembly.ExportedTypes)
                {
                    if (type.GetInterfaces().Contains(typeof(ITwoFaHandler)))
                        yield return type;
                }
            }
        }
    }
}