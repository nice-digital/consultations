// @flow
import React from "react";

type PropsType = {
	signInURL: string,
	registerURL: string,
	signInButton: boolean
}

export const LoginBanner = (props: PropsType) => (
	<div className="panel panel--inverse mt--0">
		<div className="container">
			<div className="grid">
				<div data-g="12">
					<div className="LoginBanner">
						<a href={props.signInURL} title="Sign in to your NICE account">
							Sign in to your NICE account</a> to comment on this consultation.{" "}
							Don't have an account?{" "}
						<a href={props.registerURL} title="Register for a NICE account">
							Register
						</a>
					</div>
					{props.signInButton ?
						<p>
							<a className="btn btn--inverse" href={props.signInURL} title="Sign in to your NICE account">Sign in</a>
						</p>
						:
						null
					}
				</div>
			</div>
		</div>
	</div>
);

