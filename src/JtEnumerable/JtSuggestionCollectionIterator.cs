﻿using Aadev.JTF.CustomSources;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;

namespace Aadev.JTF.JtEnumerable
{
    internal sealed class JtSuggestionCollectionIterator<T> : JtIterator<IJtSuggestionCollectionChild<T>>
    {
        private readonly JtValue owner;
        private readonly JArray source;
        private int index = -1;
        public JtSuggestionCollectionIterator(JtValue owner, JArray source)
        {
            this.owner = owner;
            this.source = source;
        }


        public override JtIterator<IJtSuggestionCollectionChild<T>> Clone() => new JtSuggestionCollectionIterator<T>(owner, source);
        public override bool MoveNext()
        {
            index++;
            if (index >= source.Count)
            {
                Current = null!;
                return false;
            }
            Current = CreateSuggestionItem(source[index]);
            return true;
        }

        private IJtSuggestionCollectionChild<T> CreateSuggestionItem(JToken source)
        {
            if (source is null)
                return new JtSuggestion<T>(default!, "Unknown");
            if (source.Type is JTokenType.Array || source.Type is JTokenType.String)
            {
                return JtSuggestionCollection<T>.Create(owner, source);
            }
            if (source.Type is JTokenType.Object)
            {
                return new JtSuggestion<T>((JObject)source);
            }
            return new JtSuggestion<T>(default!, "Unknown");
        }
    }
}