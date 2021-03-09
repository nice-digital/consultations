import React, { Component } from "react";
import Moment from "react-moment";
import { Link } from "react-router-dom";
import { tagManager } from "../../helpers/tag-manager";

export class SubmittedContent extends Component {

	render() {
		const {
			organisationName,
			isOrganisationCommenter,
			isLead,
			consultationState,
			consultationId,
			basename,
			isSubmitted,
			linkToReviewPage,
		} = this.props;

		return (
			<>
				{ isSubmitted &&
                   	<h2>Your response was submitted {isOrganisationCommenter  && !isLead && `to ${organisationName}`} on <Moment format="D MMMM YYYY" date={consultationState.submittedDate}/>.</h2>
				}
														
				<ul className="list list--unstyled">
					{linkToReviewPage &&
                        <li>
                        	<Link
                        		to={`/${consultationId}/review`}
                        		data-qa-sel="review-submitted-comments">
                                Review your response
                        	</Link>
                        </li>
					}
					{isSubmitted && consultationState.supportsDownload &&
                    <li>
                    	<a
                    		onClick={() => {
                    			tagManager({
                    				event: "generic",
                    				category: "Consultation comments page",
                    				action: "Clicked",
                    				label: "Download your response button",
                    			});
                    		}}
                    		href={`${basename}/api/exportexternal/${consultationId}`}>
                            Download submitted response (Excel)</a>
                    </li>
					}
					{isLead && consultationState.leadHasBeenSentResponse &&
                    <li>
                    	<a
                    		onClick={() => {
                    			tagManager({
                    				event: "generic",
                    				category: "Consultation comments page",
                    				action: "Clicked",
                    				label: "Download comments submitted to lead button",
                    			});
                    		}}
                    		href={`${basename}/api/exportlead/${consultationId}`}>
                        Download all responses from your organisation (Excel)</a>
                    </li>
					}
				</ul>

				{isSubmitted &&
                    <>
                    	<h2>What happens next?</h2>
                    	{isOrganisationCommenter && !isLead ? (
                    		<>
                    			<p>{`${organisationName}`} will review all the submissions received for this consultation.</p>
                    			<p>NICE's response to all the submissions received will be published on the website around the time the final guidance is published.</p>
                    		</>
                    	) : (
                    		<p>We will review all the submissions received for this consultation. Our response	will be published on the website around the time the guidance is published.</p>
                    	)}
                    	<hr/>
                    </>
				}
			</>
		);
	}
}
