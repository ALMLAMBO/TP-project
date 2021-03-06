﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TpProject.Models.Data {
	[Table("tblCourses")]
	public class CourseDTO {
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Slug { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public string CategoryName { get; set; }
		public int CategoryId { get; set; }
		public string VideoName { get; set; }
		public DateTime CreatedAt { get; set; }

		[ForeignKey("CategoryId")]
		public virtual CategoryDTO Category { get; set; }
	}
}