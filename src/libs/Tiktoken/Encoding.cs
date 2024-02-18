﻿using System.Diagnostics;
using Tiktoken.Models;
using Tiktoken.Services;

namespace Tiktoken;

/// <summary>
/// 
/// </summary>
public class Encoding
{
    /// <summary>
    /// Returns encoding by model name.
    /// </summary>
    /// <param name="modelName">gpt-3.5-turbo</param>
    /// <returns></returns>
    public static Encoding ForModel(string modelName)
    {
        return Get(Helpers.GetNameByModel(modelName));
    }
    
    /// <summary>
    /// Returns encoding by model name or null.
    /// </summary>
    /// <param name="modelName">gpt-3.5-turbo</param>
    /// <returns></returns>
    public static Encoding? TryForModel(string modelName)
    {
        var encodingName = Helpers.TryGetNameByModel(modelName);
        
        return encodingName == null
            ? null
            : Get(encodingName);
    }

    /// <summary>
    /// Returns encoding by name.
    /// </summary>
    /// <param name="encodingName">cl100k_base</param>
    /// <returns></returns>
    public static Encoding Get(string encodingName)
    {
        if (string.IsNullOrEmpty(encodingName))
        {
            throw new ArgumentException("encodingName is null or empty", nameof(encodingName));
        }

        var setting = EncodingManager.Get(encodingName);
        
        return new Encoding(setting);
    }

    private readonly CoreBpe _corePbe;
    private readonly HashSet<string> _specialTokensSet;
    private static readonly HashSet<string> emptyHashSet = new HashSet<string>();
    
    /// <summary>
    /// Enable cache for fast encoding.
    /// Default: true.
    /// </summary>
    public bool EnableCache
    {
        get => _corePbe.EnableCache;
        set => _corePbe.EnableCache = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="setting"></param>
    public Encoding(EncodingSettingModel setting)
    {
        setting = setting ?? throw new ArgumentNullException(nameof(setting));
        
        if (setting.ExplicitNVocab != null)
        {
            Debug.Assert(setting.SpecialTokens.Count + setting.MergeableRanks.Count == setting.ExplicitNVocab);
            Debug.Assert(Math.Max(setting.MergeableRanks.Values.Max(), setting.SpecialTokens.Values.Max()) == setting.ExplicitNVocab - 1);
        }

        _corePbe = new CoreBpe(setting.MergeableRanks, setting.SpecialTokens, setting.Pattern);
        _specialTokensSet = new HashSet<string>(setting.SpecialTokens.Keys);
    }

    /// <summary>
    /// Counts tokens in fast mode. Does not take into account special tokens.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public int CountTokens(string text)
    {
        return _corePbe.CountTokensNative(text);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public IReadOnlyCollection<int> Encode(string text)
    {
        return EncodeWithAllDisallowedSpecial(text);
    }
    
    /// <summary>
    /// Returns tokens from the processing stage as a list of strings.
    /// This would enhance visibility over the tokenization process, facilitate token manipulation,
    /// and could serve as a useful tool for educational purposes.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public IReadOnlyCollection<string> Explore(string text)
    {
        return _corePbe.Explore(
            text,
            allowedSpecial: _specialTokensSet,
            disallowedSpecial: emptyHashSet);
    }
    
    /// <summary>
    /// Returns tokens from the processing stage as a list of strings.
    /// This would enhance visibility over the tokenization process, facilitate token manipulation,
    /// and could serve as a useful tool for educational purposes.
    /// Unlike <see cref="Explore"/> this method returns token in a printable manner, in which each token is encoded as one more tokens.
    /// For example, <see cref="Encodings.Cl100KBase"/> can encode 🤚🏾 (Raised Back of Hand: Dark Skin Tone) with as much as 6 tokens.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public IReadOnlyCollection<UtfToken> ExploreUtfSafe(string text)
    {
        return _corePbe.ExploreUtfSafe(
            text,
            allowedSpecial: _specialTokensSet,
            disallowedSpecial: emptyHashSet);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public IReadOnlyCollection<int> EncodeWithAllAllowedSpecial(string text)
    {
        return _corePbe.EncodeNative(
            text,
            allowedSpecial: _specialTokensSet,
            disallowedSpecial: emptyHashSet);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public IReadOnlyCollection<int> EncodeWithAllDisallowedSpecial(string text)
    {
        return _corePbe.EncodeNative(
            text,
            allowedSpecial: emptyHashSet,
            disallowedSpecial: _specialTokensSet);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="allowedSpecial"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public IReadOnlyCollection<int> EncodeWithAllowedSpecial(
        string text,
        IReadOnlyCollection<string> allowedSpecial)
    {
        allowedSpecial = allowedSpecial ?? throw new ArgumentNullException(nameof(allowedSpecial));
        
        return _corePbe.EncodeNative(
            text,
            allowedSpecial: new HashSet<string>(allowedSpecial),
            disallowedSpecial: new HashSet<string>(_specialTokensSet.Except(allowedSpecial)));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="disallowedSpecial"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public IReadOnlyCollection<int> EncodeWithDisallowedSpecial(
        string text,
        IReadOnlyCollection<string> disallowedSpecial)
    {
        disallowedSpecial = disallowedSpecial ?? throw new ArgumentNullException(nameof(disallowedSpecial));
        
        return _corePbe.EncodeNative(
            text,
            allowedSpecial: new HashSet<string>(_specialTokensSet.Except(disallowedSpecial)),
            disallowedSpecial: new HashSet<string>(disallowedSpecial));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns></returns>
    public string Decode(IReadOnlyCollection<int> tokens)
    {
        var bytes = _corePbe.DecodeNative(tokens);
        
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}