﻿using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    [HtmlTargetElement("nop-nested-setting", Attributes = ForAttributeName)]
    public class NopNestedSettingTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        /// <summary>
        /// HtmlGenerator
        /// </summary>
        protected IHtmlGenerator Generator { get; set; }

        /// <summary>
        /// An expression to be evaluated against the current model
        /// </summary>
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public NopNestedSettingTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var parentSettingName = For.Name;

            var random = CommonHelper.GenerateRandomInteger();
            var nestedSettingId = $"nestedSetting{random}";
            var parentSettingId = $"parentSetting{random}";

            //tag details
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "nested-setting");
            output.Attributes.Add("id", nestedSettingId);

            //script
            var script = new TagBuilder("script");
            script.InnerHtml.AppendHtml("$(document).ready(function () {" +
                                            $"var parentFormGroup = $('input[name=\"{parentSettingName}\"]').closest('.form-group');" +
                                            $"var parentFormGroupId = $(parentFormGroup).attr('id');" +
                                            $"if(!parentFormGroupId){{parentFormGroupId = '{parentSettingId}'}}" +
                                            $"$(parentFormGroup).addClass('parent-setting').attr('id', parentFormGroupId);" +
                                            $"if($('#{nestedSettingId} .form-group').length == $('#{nestedSettingId} .form-group.advanced-setting').length){{$('#' + parentFormGroupId).addClass('parent-setting-advanced')}}" +
                                            $"function toggleNestedSetting() {{if ($('input[name=\"{parentSettingName}\"]').is(':checked')) {{$('#' + parentFormGroupId).addClass('opened')}} else {{$('#' + parentFormGroupId).removeClass('opened')}}}}" +
                                            $"$('input[name=\"{parentSettingName}\"]').click(toggleNestedSetting);" +
                                            "toggleNestedSetting();" +
                                        "});");
            output.PreContent.SetHtmlContent(script.RenderHtmlContent());
        }
    }
}