namespace HelpTab
{
    public interface IHelpDefView
    {
		/// <summary>
		/// Handles jumping the GUI to a specific helpDef
		/// from a ResearchProjectDef
		/// </summary>
		/// <param name="def"></param>
		void JumpTo (Verse.Def def);

        /// <summary>
        /// Handles jumping the GUI to a specific helpDef
        /// </summary>
        /// <param name="def"></param>
        void JumpTo(HelpDef def);

        /// <summary>
        /// Does this view accept links to this helpDef
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        bool Accept(HelpDef def);

        /// <summary>
        /// What other view should links be redirected to. Only called when Accept() returns false.
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        IHelpDefView SecondaryView(HelpDef def);
    }
}
