// @flow
import React from "react";

type PropsType = {
	signInURL: string,
	registerURL: string
}

export const LoginBanner = (props: PropsType) => (
	<div className="panel panel--inverse mt--0">
		<div className="container">
			<div className="grid">
				<div data-g="12">
					<div className="LoginBanner">
						To comment on this consultation, <a href={props.signInURL} title="Sign in to your NICE account">sign in to your NICE account</a> or <a href={props.registerURL} title="Register for a NICE account">register for a NICE account</a> if you don't yet have one.
					</div>
					{props.signinButton ?
						<p>
							<a className="btn btn--inverse" href={props.signInURL} title="Sign in to your NICE account">Sign In</a>
						</p>
						:
						null
					}
				</div>
			</div>
		</div>
	</div>
);

