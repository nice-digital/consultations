export function createCourse(course) {
	alert("createCourse called");
	return {
		type: "CREATE_COURSE",
		course: course
	};
}
