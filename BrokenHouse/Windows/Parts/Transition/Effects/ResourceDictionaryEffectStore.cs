using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BrokenHouse.Internal;
using BrokenHouse.Utils;
using BrokenHouse.Windows.Extensions;
using BrokenHouse.Windows.Parts.Transition;


namespace BrokenHouse.Windows.Parts.Transition.Effects
{
    /// <summary>
    /// Provides an object to hold a reference to all the key components of a <see cref="System.Windows.ResourceDictionary"/>
    /// based <see cref="TransitionEffect"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It is anticipated that this class be used to create static objects that can then be referenced by a
    /// custom <see cref="TransitionEffect">TransitionEffects</see>.
    /// </para>
    /// </remarks>
    public class ResourceDictionaryEffectStore
    {
        private CompoundDictionary<TransitionPosition, Storyboard> m_TransitionStoryboards = new CompoundDictionary<TransitionPosition, Storyboard>();
        private CompoundDictionary<TransitionPosition, Style>      m_TransitionStyles      = new CompoundDictionary<TransitionPosition, Style>();
        private Dictionary<TransitionPosition, Style>              m_StaticStyles          = new Dictionary<TransitionPosition, Style>();
        private Style                                              m_EmptyStyle            = new Style(typeof(TransitionFrame));

        /// <summary>
        /// Create an effect store using the supplied uri.
        /// </summary>
        /// <param name="uri"></param>
        public ResourceDictionaryEffectStore( string uri ) : this(ResourceHelper.FindDictionary(uri))
        {
        }

        /// <summary>
        /// Create an effect store using the supplied dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        public ResourceDictionaryEffectStore( ResourceDictionary dictionary )
        { 
            m_StaticStyles[TransitionPosition.Start]                                      = GetResource<Style>(dictionary, "STYLE_AtStart", "STYLE_Default");
            m_TransitionStyles[TransitionPosition.Start, TransitionPosition.Center]       = GetResource<Style>(dictionary, "STYLE_StartToCenter", "STYLE_AtStart", "STYLE_Default");
            m_TransitionStyles[TransitionPosition.Center, TransitionPosition.Start]       = GetResource<Style>(dictionary, "STYLE_CenterToStart", "STYLE_AtCenter", "STYLE_Default");
            m_StaticStyles[TransitionPosition.Center]                                     = GetResource<Style>(dictionary, "STYLE_AtCenter", "STYLE_Default");
            m_TransitionStyles[TransitionPosition.Center, TransitionPosition.End]         = GetResource<Style>(dictionary, "STYLE_CenterToEnd", "STYLE_AtCenter", "STYLE_Default");
            m_TransitionStyles[TransitionPosition.End,    TransitionPosition.Center]      = GetResource<Style>(dictionary, "STYLE_EndToCenter", "STYLE_AtEnd", "STYLE_Default");
            m_StaticStyles[TransitionPosition.End]                                        = GetResource<Style>(dictionary, "STYLE_AtEnd", "STYLE_Default");
            m_TransitionStoryboards[TransitionPosition.Start,  TransitionPosition.Center] = GetResource<Storyboard>(dictionary, "TRANS_StartToCenter",  "TRANS_ToCenter", "TRANS_Default");
            m_TransitionStoryboards[TransitionPosition.Center, TransitionPosition.Start]  = GetResource<Storyboard>(dictionary, "TRANS_CenterToStart",  "TRANS_ToStart", "TRANS_Default");
            m_TransitionStoryboards[TransitionPosition.Center, TransitionPosition.End]    = GetResource<Storyboard>(dictionary, "TRANS_CenterToEnd",  "TRANS_ToEnd", "TRANS_Default");
            m_TransitionStoryboards[TransitionPosition.End,    TransitionPosition.Center] = GetResource<Storyboard>(dictionary, "TRANS_EndToCenter",  "TRANS_ToCenter", "TRANS_Default");

        }

        /// <summary>
        /// Internal function to obtain a object based on a list of keys.
        /// </summary>
        /// <remarks>
        /// The keys are used, in order, to search for a named object in the <see cref="System.Windows.ResourceDictionary"/>. If all the keys
        /// are exhausted then null is returned.
        /// </remarks>
        /// <typeparam name="T">The type of object to return from the <see cref="System.Windows.ResourceDictionary"/>.</typeparam>
        /// <param name="dictionary">The <see cref="System.Windows.ResourceDictionary"/> to search.</param>
        /// <param name="keys">The list of keys to use in the search.</param>
        /// <returns>An object from the <see cref="System.Windows.ResourceDictionary"/>. </returns>
        private static T GetResource<T>( ResourceDictionary dictionary, params String[] keys )
        {
            return keys.Select(k => (T)dictionary[k]).Where(v => v != null).FirstOrDefault();
        }
 
        /// <summary>
        /// Obtain a storyboard for the required animation.
        /// </summary>
        /// <param name="startPosition">The starting position of the animation.</param>
        /// <param name="endPosition">The target position of the animation.</param>
        /// <returns>The storyboard that will perform the animation.</returns>
        public Storyboard GetStoryboard( TransitionPosition startPosition, TransitionPosition endPosition )
        {
            Storyboard storyboard = m_TransitionStoryboards[startPosition, endPosition];

            return (storyboard == null)? null : storyboard.Clone();
        }

        /// <summary>
        /// Obtain a style that will put a <see cref="TransitionFrame"/> at the required position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Style GetStyle( TransitionPosition position )
        {
            return m_StaticStyles[position];
        }


        /// <summary>
        /// Obtain a style that will, prepare 
        /// a <see cref="TransitionFrame"/> to be animated from the <paramref name="startPosition"/> to the 
        /// <paramref name="endPosition"/>.
        /// </summary>
        /// <param name="startPosition">The position that we will aniamate from.</param>
        /// <param name="endPosition">The position that we will aniamate to.</param>
        /// <returns>The style that will prepare the <see cref="TransitionFrame"/>.</returns>
        public Style GetStyle( TransitionPosition startPosition, TransitionPosition endPosition )
        {
            return m_TransitionStyles[startPosition, endPosition];
        }
    }
}
