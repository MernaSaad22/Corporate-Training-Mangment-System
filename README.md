# 🏢 Corporate Training Management System (SaaS Platform)

🎬 **[Watch the Full Backend End-to-End Demo Here](https://drive.google.com/drive/folders/1lExlaBxOaVj77N14TRfVSBWJ51zAHkbo)**

A massive, multi-tenant SaaS (Software as a Service) platform built with **.NET 9** and **Clean Architecture**. This enterprise-grade API is designed to revolutionize how companies manage employee learning, track progress, and evaluate performance. 

Engineered with complex business rules, real-time algorithmic progress tracking, automated grading, and a highly secure Role-Based Access Control (RBAC) system.

---

## 🚀 Core Engineering Masterpieces

### 1. 🧠 Algorithmic Progress Tracking Engine
Engineered a highly mathematical `ProgressService` that autonomously calculates and updates an employee's learning curve in real-time:
- **Weighted Completion:** Overall course progress is strictly calculated dynamically: **Lessons (50%)**, **Assignments (25%)**, and **Exams (25%)**.
- **Automated Graduation:** The system continuously listens to the employee's progress. Once the total score hits **85%** (before the assigned deadline), the system automatically locks the course and marks it as officially `Completed`.

### 2. 🏢 B2B SaaS Multi-Tenancy & Stripe Billing
Built a complete subscription-based architecture for companies:
- **Plan Enforcement:** The database strictly prevents a `CompanyAdmin` from exceeding their purchased plan's `MaxEmployees` and `MaxCourses` limits.
- **Subscription Lifecycle:** Integrated **Stripe API** for handling payments. The system intelligently handles first-time subscriptions, renewals, and upgrades, dynamically calculating the `StartDate` and `EndDate`.
- **Tenant Isolation:** Every course, category, and employee is strictly isolated. Instructors and employees can only access data strictly belonging to their localized company.

### 3. 🛡️ Strict Deadlines & Automated Grading
- **Exam Engine:** Employees submit exams which are **Auto-Graded** instantly by the system comparing submitted answers against encrypted correct options.
- **Time-Locked Endpoints:** Strict validation ensures that `ExamSubmissions` and `AssignmentSubmissions` throw forbidden errors if the `DateTime.UtcNow` exceeds the instructor-defined deadlines.

### 4. 📂 Advanced Media & File Management
- Built robust file validation and processing for **100MB+ Video Uploads** for lessons, **PDF/Docx** for assignment submissions, and company logos.
- Handled secure physical file deletion from the server upon entity cascading deletion to prevent memory leaks and orphaned files.

### 5. 🔄 Dynamic Entity Reordering
- Engineered complex `Drag-and-Drop` APIs for `Chapters` and `Lessons` allowing instructors to dynamically shift the `Order` index of entities. The algorithm automatically shifts remaining entities up or down to fill gaps seamlessly.

---

## 👑 Comprehensive Role-Based Access Control (RBAC)

The system features 4 strictly isolated roles authenticated via **JWT Tokens**:

### 👨‍💻 1. SuperAdmin
- Full overview of the SaaS ecosystem.
- Creates and manages Subscription `Plans` (Cost, Duration, Quotas).
- Audits registered `Companies`.

### 🏢 2. CompanyAdmin
- Securely registers the company and processes Stripe payments to activate the tenant space.
- Full CRUD over `Employees` and `Instructors` (strictly within plan limits).
- Assigns courses to specific employees with strict `DeadlineDays` for completion.
- Tracks real-time analytical progress for all employees via the Progress Engine.

### 👨‍🏫 3. Instructor
- Architects `Courses`, `Chapters`, and `Lessons` (with video content uploads).
- Can mathematically reorder chapters and lessons anytime.
- Creates `Assignments` (with downloadable attachments) and sets strict deadlines.
- Creates `Exams` with Multiple Choice Questions.
- Manually reviews and grades employee assignment file submissions.

### 👨‍💼 4. Employee
- Browses assigned courses and tracks personal completion status.
- System automatically registers `ViewedAt` timestamps when lessons are opened.
- Submits assignments (PDF/Docs) securely before deadlines.
- Takes timed exams that are instantly auto-graded by the backend.

---

## 💻 Technical Stack & Architecture

- **Framework:** .NET 9, ASP.NET Core Web API
- **Architecture:** Clean Architecture, Generic Repository Pattern, Unit of Work
- **Database:** Microsoft SQL Server, Entity Framework Core (Code-First)
- **Security:** ASP.NET Core Identity, JWT Authentication, Custom Authorization Policies
- **Data Mapping:** Mapster (High-performance object mapping)
- **Payment Gateway:** Stripe API integration
- **Email Service:** Custom SMTP `IEmailSender` for Email Confirmations and Password Resets
- **Documentation:** Scalar / OpenAPI

---

## 🏗️ Database Design Highlights
The database schema uses advanced **Fluent API** configurations:
- **Restricted Cascading:** Applied `DeleteBehavior.Restrict` across critical entities to prevent accidental data wiping (e.g., deleting a company won't silently delete all users).
- **Composite Unique Keys:** Used `.HasIndex(e => new { e.EmployeeId, e.CourseId }).IsUnique()` to strictly prevent an employee from being enrolled in the same course twice or submitting the same exam multiple times.
- **Precision Types:** Enforced strict decimal precision `(18,2)` for all financial transactions and progress grading numbers.
