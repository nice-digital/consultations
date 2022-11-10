import React from "react";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { CommentBox } from "../CommentBox";
import sampleComment from "./sample.json";

const fakeProps = {
	comment: {
		commentId: sampleComment.commentId,
		commentText: "a comment-",
		lastModifiedDate: new Date("01/04/2018").toISOString(),
	},
	updateUnsavedIds: jest.fn(),
};

test("sets text area with comment text correctly", () => {
	render(<CommentBox {...fakeProps} />);
	expect(screen.getByDisplayValue("a comment-")).toBeInTheDocument();
});

test("unsavedChanges message appears on text area change", async () => {
	render(<CommentBox {...fakeProps} />);
	const textArea = screen.getByDisplayValue("a comment-");
	const user = userEvent.setup();
	textArea.focus();
	await user.type(textArea, "that's been updated");
	user.tab();
	const unsavedChanges = await screen.findByText("You have unsaved changes", { selector: "p" });
	expect(screen.getByDisplayValue("a comment-that's been updated")).toBeInTheDocument();
	expect(unsavedChanges).toBeInTheDocument();
});
