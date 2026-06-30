import os
import re
import math
from typing import Dict, List, Tuple

# ==========================================================================
# 1. DATA DEFINITIONS & SCHEMAS
# ==========================================================================

# Pre-defined Academic Courses
COURSES = {
    "CS101": "Data Structures & Algorithms",
    "CS102": "Object-Oriented Programming (OOP)",
    "CS201": "Database Management Systems (DBMS)",
    "CS202": "Web Application Development",
    "CS301": "Operating Systems & Networking",
    "CS302": "Software Engineering & Devops Lab",
    "CS401": "Machine Learning & Data Mining",
}

# Professional Competencies
COMPETENCIES = [
    "Algorithmic Optimization",
    "OOP & Design Patterns",
    "Database Tuning & SQL",
    "Frontend Tech (React/HTML/CSS)",
    "Backend REST APIs (FastAPI/Node)",
    "System Concurrency & Network",
    "CI/CD Pipelines & Docker",
    "Machine Learning & Data Math"
]

# Course-to-Competency Mapping Weights (Graph edges)
ACADEMIC_MAP = {
    "CS101": {"Algorithmic Optimization": 0.9, "OOP & Design Patterns": 0.3},
    "CS102": {"OOP & Design Patterns": 0.9, "Backend REST APIs (FastAPI/Node)": 0.2},
    "CS201": {"Database Tuning & SQL": 0.9, "Backend REST APIs (FastAPI/Node)": 0.4},
    "CS202": {"Frontend Tech (React/HTML/CSS)": 0.8, "Backend REST APIs (FastAPI/Node)": 0.8},
    "CS301": {"System Concurrency & Network": 0.8, "Algorithmic Optimization": 0.3},
    "CS302": {"CI/CD Pipelines & Docker": 0.9, "OOP & Design Patterns": 0.4},
    "CS401": {"Machine Learning & Data Math": 0.9, "Database Tuning & SQL": 0.3}
}

# Career Profile Vectors (Required levels from 0.0 to 10.0)
CAREER_PROFILES = {
    "Backend Developer": {
        "Algorithmic Optimization": 8.0,
        "OOP & Design Patterns": 8.5,
        "Database Tuning & SQL": 9.0,
        "Frontend Tech (React/HTML/CSS)": 4.0,
        "Backend REST APIs (FastAPI/Node)": 9.5,
        "System Concurrency & Network": 7.5,
        "CI/CD Pipelines & Docker": 7.0,
        "Machine Learning & Data Math": 3.0
    },
    "Frontend Developer": {
        "Algorithmic Optimization": 6.0,
        "OOP & Design Patterns": 7.5,
        "Database Tuning & SQL": 4.0,
        "Frontend Tech (React/HTML/CSS)": 9.5,
        "Backend REST APIs (FastAPI/Node)": 6.5,
        "System Concurrency & Network": 4.5,
        "CI/CD Pipelines & Docker": 5.5,
        "Machine Learning & Data Math": 2.0
    },
    "Data Scientist / ML Engineer": {
        "Algorithmic Optimization": 8.5,
        "OOP & Design Patterns": 7.0,
        "Database Tuning & SQL": 8.0,
        "Frontend Tech (React/HTML/CSS)": 3.0,
        "Backend REST APIs (FastAPI/Node)": 6.0,
        "System Concurrency & Network": 5.0,
        "CI/CD Pipelines & Docker": 6.0,
        "Machine Learning & Data Math": 9.5
    },
    "DevOps Engineer": {
        "Algorithmic Optimization": 6.0,
        "OOP & Design Patterns": 6.0,
        "Database Tuning & SQL": 6.5,
        "Frontend Tech (React/HTML/CSS)": 3.5,
        "Backend REST APIs (FastAPI/Node)": 7.0,
        "System Concurrency & Network": 8.5,
        "CI/CD Pipelines & Docker": 9.5,
        "Machine Learning & Data Math": 3.0
    }
}

# Stopwords for simple TF-IDF keyword extraction
STOPWORDS = {
    'i', 'me', 'my', 'myself', 'we', 'our', 'ours', 'ourselves', 'you', 'your', 'yours', 'he', 'him', 'his', 'she', 
    'her', 'it', 'its', 'they', 'them', 'their', 'what', 'which', 'who', 'whom', 'this', 'that', 'these', 'those', 
    'am', 'is', 'are', 'was', 'were', 'be', 'been', 'being', 'have', 'has', 'had', 'having', 'do', 'does', 'did', 
    'a', 'an', 'the', 'and', 'but', 'if', 'or', 'because', 'as', 'until', 'while', 'of', 'at', 'by', 'for', 'with', 
    'about', 'against', 'between', 'into', 'through', 'during', 'before', 'after', 'above', 'below', 'to', 'from', 
    'in', 'out', 'on', 'off', 'over', 'under', 'then', 'once', 'here', 'there', 'when', 'where', 'why', 'how', 'all'
}

# Action verbs mapped to career domains
ACTION_VERBS = {
    "Backend Developer": ["Optimized", "Architected", "Engineered", "Integrated", "Built"],
    "Frontend Developer": ["Designed", "Crafted", "Implemented", "Enhanced", "Revamped"],
    "Data Scientist / ML Engineer": ["Analyzed", "Modeled", "Trained", "Extracted", "Predicted"],
    "DevOps Engineer": ["Automated", "Deployed", "Streamlined", "Configured", "Containerized"]
}

# ==========================================================================
# 2. ALGORITHM IMPLEMENTATIONS
# ==========================================================================

def clean_and_tokenize(text: str) -> List[str]:
    """Lowercase and extract alphanumeric words."""
    return re.findall(r'\b[a-z0-9\-\+\#]{2,}\b', text.lower())

def extract_tfidf_keywords(jd: str, corpus: List[str], top_n: int = 8) -> List[str]:
    """Calculate term frequencies vs document frequencies to extract JD keywords."""
    jd_words = [w for w in clean_and_tokenize(jd) if w not in STOPWORDS]
    if not jd_words:
        return []
        
    tf = {}
    for w in jd_words:
        tf[w] = tf.get(w, 0) + 1
        
    # Count document frequency from corpus
    df = {}
    all_docs = corpus + [jd]
    for doc in all_docs:
        doc_words = set(clean_and_tokenize(doc))
        for w in tf:
            if w in doc_words:
                df[w] = df.get(w, 0) + 1
                
    tfidf = {}
    n_docs = len(all_docs)
    for w, freq in tf.items():
        doc_freq = df.get(w, 1)
        idf = math.log(n_docs / doc_freq)
        tfidf[w] = (freq / len(jd_words)) * idf
        
    sorted_kws = sorted(tfidf.items(), key=lambda x: x[1], reverse=True)
    return [kw for kw, score in sorted_kws[:top_n]]

def compute_cosine_similarity(vec1: Dict[str, float], vec2: Dict[str, float]) -> float:
    """Calculate the Cosine Similarity metric between two vectors."""
    dot_product = 0.0
    norm_a = 0.0
    norm_b = 0.0
    
    all_keys = set(vec1.keys()).union(set(vec2.keys()))
    for key in all_keys:
        val1 = vec1.get(key, 0.0)
        val2 = vec2.get(key, 0.0)
        dot_product += val1 * val2
        norm_a += val1 ** 2
        norm_b += val2 ** 2
        
    if norm_a == 0.0 or norm_b == 0.0:
        return 0.0
    return dot_product / (math.sqrt(norm_a) * math.sqrt(norm_b))

# ==========================================================================
# 3. CORE FEATURES IMPLEMENTATION
# ==========================================================================

# [1 & 2] Academic-to-Career Mapping & Career Recommendations
def analyze_student_profile(grades: Dict[str, float]) -> Tuple[Dict[str, float], List[dict]]:
    # Map grades (0-10) to competencies (0-10) using weighted edges
    student_comp = {comp: 0.0 for comp in COMPETENCIES}
    weights_sum = {comp: 0.0 for comp in COMPETENCIES}
    
    for course, grade in grades.items():
        if course in ACADEMIC_MAP:
            for comp, weight in ACADEMIC_MAP[course].items():
                student_comp[comp] += grade * weight
                weights_sum[comp] += weight
                
    for comp in COMPETENCIES:
        if weights_sum[comp] > 0:
            student_comp[comp] = min(student_comp[comp] / weights_sum[comp], 10.0)
            
    # Match careers via Cosine Similarity
    recommendations = []
    for role, req_vec in CAREER_PROFILES.items():
        sim = compute_cosine_similarity(student_comp, req_vec)
        
        # Calculate gaps
        gaps = []
        for comp, req_val in req_vec.items():
            stu_val = student_comp[comp]
            if stu_val < req_val:
                gaps.append({"competency": comp, "gap": round(req_val - stu_val, 2)})
        gaps = sorted(gaps, key=lambda x: x["gap"], reverse=True)
        
        recommendations.append({
            "role": role,
            "similarity": round(sim * 100, 1),
            "gaps": gaps[:2]
        })
        
    recommendations = sorted(recommendations, key=lambda x: x["similarity"], reverse=True)
    return student_comp, recommendations

# [3] AI Resume Generation: STAR / XYZ Rewrite
def optimize_bullet_star(raw_bullet: str, target_role: str) -> str:
    verbs = ACTION_VERBS.get(target_role, ["Spearheaded", "Optimized", "Designed"])
    clean_bullet = re.sub(r'^(i was responsible for |responsible for |i worked on |worked on |helped to |helped in )', '', raw_bullet, flags=re.IGNORECASE)
    
    if clean_bullet:
        clean_bullet = clean_bullet[0].lower() + clean_bullet[1:]
        
    # Heuristics based on role to add specific technology and simulated outcome metrics
    tech = "advanced logic"
    metric = "25%"
    if target_role == "Backend Developer":
        tech = "FastAPI endpoints and SQL indexing"
        metric = "35% faster query response time"
    elif target_role == "Frontend Developer":
        tech = "React responsive components and clean state layout"
        metric = "40% increase in mobile UX scores"
    elif target_role == "DevOps Engineer":
        tech = "Docker multi-stage builds and GitHub Actions CI/CD pipelines"
        metric = "50% reduction in deployment pipeline duration"
        
    return f"{verbs[0]} and deployed {clean_bullet} utilizing {tech}, achieving a {metric}."

# [4] Resume Template Generation (HTML Export)
def generate_html_resume(filepath: str, name: str, title: str, email: str, skills: List[str], education: str, bullet: str, template: str = "modern"):
    skills_html = "".join([f'<span class="skill-tag">{s}</span>' for s in skills])
    
    # CSS template variables
    styles = ""
    if template == "modern":
        styles = """
            body { font-family: 'Inter', sans-serif; background: #0b0f19; color: #f3f4f6; padding: 40px; }
            .cv-card { max-width: 800px; margin: 0 auto; background: #111827; border-top: 8px solid #6366f1; padding: 40px; border-radius: 12px; box-shadow: 0 4px 20px rgba(0,0,0,0.5); }
            h1 { font-family: 'Outfit', sans-serif; font-size: 2.2rem; color: #fff; margin-bottom: 5px; }
            .title-role { color: #818cf8; font-size: 1.1rem; font-weight: 600; margin-bottom: 20px; }
            h2 { font-size: 1.2rem; border-bottom: 1px solid #374151; padding-bottom: 5px; margin-top: 30px; color: #fff; }
            .skill-tag { display: inline-block; background: rgba(99,102,241,0.15); border: 1px solid rgba(99,102,241,0.3); color: #c7d2fe; padding: 4px 10px; border-radius: 6px; margin: 5px; font-size: 0.85rem; }
        """
    else:  # Minimalist template style
        styles = """
            body { font-family: Georgia, serif; background: #fff; color: #222; padding: 40px; }
            .cv-card { max-width: 800px; margin: 0 auto; border: 1px solid #ddd; padding: 40px; }
            h1 { text-align: center; font-size: 2rem; margin-bottom: 5px; }
            .title-role { text-align: center; font-style: italic; color: #555; margin-bottom: 20px; }
            h2 { font-size: 1.1rem; border-bottom: 2px solid #222; padding-bottom: 2px; margin-top: 25px; text-transform: uppercase; }
            .skill-tag { display: inline-block; color: #222; margin-right: 15px; font-weight: bold; }
        """

    html_content = f"""<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <title>{name} - Resume</title>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;600;700&family=Outfit:wght@700&display=swap" rel="stylesheet">
    <style>
        {styles}
        .contacts {{ margin-bottom: 20px; font-size: 0.9rem; color: #9ca3af; }}
        ul {{ margin-left: 20px; line-height: 1.6; }}
        li {{ margin-bottom: 8px; }}
    </style>
</head>
<body>
    <div class="cv-card">
        <h1>{name}</h1>
        <div class="title-role">{title}</div>
        <div class="contacts">Email: {email} | GitHub: github.com/traductoan</div>
        
        <h2>Education</h2>
        <p>{education}</p>
        
        <h2>Core Skills</h2>
        <div style="margin: 10px 0;">{skills_html}</div>
        
        <h2>Experience & Projects</h2>
        <ul>
            <li>{bullet}</li>
        </ul>
    </div>
</body>
</html>
"""
    with open(filepath, "w", encoding="utf-8") as f:
        f.write(html_content)

# [5 & 6] ATS Optimization & Personalized Resume Generation
def audit_ats_compatibility(resume_text: str, jd_text: str) -> dict:
    # Simulating standard job descriptions as a DF reference corpus
    corpus = [
        "We need a backend developer with skills in Python, FastAPI, Docker, and SQL tuning.",
        "Frontend engineer role focused on React, HTML, CSS, JavaScript, and responsive design.",
        "Devops engineer vacancy. Proficient in Git, Docker containerization, and deployment pipelines."
    ]
    
    # Extract top keywords using local TF-IDF
    jd_kws = extract_tfidf_keywords(jd_text, corpus, top_n=6)
    
    # Scan resume for matched keywords
    matched = []
    missing = []
    res_lower = resume_text.lower()
    
    for kw in jd_kws:
        if kw in res_lower:
            matched.append(kw)
        else:
            missing.append(kw)
            
    # Scoring
    kw_score = int((len(matched) / len(jd_kws)) * 60) if jd_kws else 0
    structure_score = 20 if "education" in res_lower and "experience" in res_lower and "skills" in res_lower else 10
    total_score = kw_score + structure_score + 20 # 20 base formatting score
    
    return {
        "score": total_score,
        "matched": matched,
        "missing": missing,
        "keywords_extracted": jd_kws
    }

# ==========================================================================
# 4. DEMO RUNNER
# ==========================================================================

def main():
    print("=" * 76)
    print("      PATHFINDER AI: RESUME GENERATOR & CAREER PORTAL (SIMPLIFIED)")
    print("=" * 76)

    # ----------------------------------------------------------------------
    # STEP 1: Academic-to-Career Mapping & Recommendations
    # ----------------------------------------------------------------------
    print("\n[Bước 1] Phân tích điểm & Gợi ý nghề nghiệp (Academic & Career System)")
    print("-" * 76)
    mock_student_grades = {
        "CS101": 9.2,  # DSA (High grade)
        "CS102": 8.8,  # OOP (High grade)
        "CS201": 9.0,  # DBMS (High grade)
        "CS202": 7.0,  # Web Dev (Medium grade)
        "CS301": 6.5,  # OS (Medium grade)
        "CS302": 8.5,  # DevOps (High grade)
        "CS401": 5.0,  # ML (Low grade)
    }
    
    comp_vector, recs = analyze_student_profile(mock_student_grades)
    
    print("  * Điểm số học tập đầu vào (Thang điểm 10):")
    for c_code, grade in mock_student_grades.items():
        print(f"    - {c_code} ({COURSES[c_code]}): {grade}/10")
        
    print("\n  * Vector năng lực chuyên môn suy luận (Competency Profile):")
    for comp, val in comp_vector.items():
        bar = "█" * int(val) + "░" * (10 - int(val))
        print(f"    - {comp:<32} : {val:.1f}/10 | {bar}")

    print("\n  * Đề xuất nghề nghiệp phù hợp nhất (Cosine Similarity):")
    best_role = recs[0]["role"]
    for idx, rec in enumerate(recs[:3], 1):
        print(f"    {idx}. {rec['role']:<28} : {rec['similarity']}% tương thích")
        if rec["gaps"]:
            gaps_str = ", ".join([f"{g['competency']} (thiếu -{g['gap']})" for g in rec["gaps"]])
            print(f"       -> Lỗ hổng kỹ năng (Skill Gaps): {gaps_str}")

    # ----------------------------------------------------------------------
    # STEP 2: AI STAR/XYZ Resume Generation
    # ----------------------------------------------------------------------
    print(f"\n[Bước 2] Tối ưu hóa mô tả kinh nghiệm chuẩn STAR (Mục tiêu: {best_role})")
    print("-" * 76)
    raw_desc = "I was responsible for writing backend endpoints and speeding up query search times."
    star_desc = optimize_bullet_star(raw_desc, best_role)
    print(f"  * Mô tả thô: {raw_desc}")
    print(f"  * STAR/XYZ:  {star_desc}")

    # ----------------------------------------------------------------------
    # STEP 3: ATS Resume Optimization Audit
    # ----------------------------------------------------------------------
    print("\n[Bước 3] Đánh giá độ tương thích ATS (ATS Optimization Audit)")
    print("-" * 76)
    sample_jd = """
    We are looking for a Backend Developer. The candidate should be strong in database tuning, 
    SQL queries, Python scripting, FastAPI frameworks, and Docker deployments. 
    Collaborating with git repositories is essential.
    """
    
    # Simple text representation of candidate resume
    resume_text = f"""
    Trà Đức Toàn - Junior Backend Developer
    Summary: Computer Science student focused on database and scalable backend software engineering.
    Skills: Python, SQL, PostgreSQL, Docker, Git.
    Experience: {star_desc}
    Education: B.S. in Computer Science - University of Science.
    """
    
    ats_report = audit_ats_compatibility(resume_text, sample_jd)
    print(f"  * Job Description (Yêu cầu tuyển dụng):")
    print(f"    \"...Backend Developer... database tuning, SQL, Python, FastAPI, Docker...\"")
    print(f"\n  * Kết quả Audit ATS:")
    print(f"    - Điểm tương thích ATS: {ats_report['score']}/100")
    print(f"    - Từ khóa quan trọng trích xuất từ JD: {', '.join(ats_report['keywords_extracted'])}")
    print(f"    - Từ khóa khớp (Matched): {', '.join(ats_report['matched'])}")
    print(f"    - Từ khóa thiếu (Missing): {', '.join(ats_report['missing'])}")

    # ----------------------------------------------------------------------
    # STEP 4: Personalized Resume Template Generation (Output HTML file)
    # ----------------------------------------------------------------------
    print("\n[Bước 4] Tạo bản CV Cá nhân hóa xuất ra File (Personalized Resume Template)")
    print("-" * 76)
    
    output_filename = "resume_output.html"
    output_path = os.path.join(os.path.dirname(__file__), output_filename)
    
    # Custom skills based on matching career track
    tailored_skills = ["Python", "FastAPI", "SQL Tuning", "PostgreSQL", "Docker", "Git"] if best_role == "Backend Developer" else ["HTML", "CSS", "React", "JavaScript", "Git"]
    
    generate_html_resume(
        filepath=output_path,
        name="Trà Đức Toàn",
        title=f"Junior {best_role}",
        email="traductoan.dev@gmail.com",
        skills=tailored_skills,
        education="B.S. in Computer Science - University of Science (GPA: 3.6/4.0)",
        bullet=star_desc,
        template="modern"
    )
    
    print(f"  * Thành công: Bản CV mẫu Modern Slate đã được lưu thành file:")
    print(f"    👉 {output_path}")
    print("    (Mở file này bằng trình duyệt của bạn để xem và in bản PDF A4 chuẩn!)")
    print("=" * 76)

if __name__ == "__main__":
    main()
