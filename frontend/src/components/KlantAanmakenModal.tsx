import React, { useState } from "react";
import "../styles/Error.css";

interface Props {
    onClose: () => void;
    onSuccess: () => void;
}

const KlantAanmakenModal: React.FC<Props> = ({ onClose, onSuccess }) => {
    const [form, setForm] = useState({
        bedrijfsNaam: "",
        contactPersoon: "",
        email: "",
        telefoonNummer: "",
        adres: "",
    });

    const [errors, setErrors] = useState({
        bedrijfsNaam: "",
        contactPersoon: "",
        email: "",
        telefoonNummer: "",
        adres: "",
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setForm({ ...form, [name]: value });
        setErrors({ ...errors, [name]: "" });
    };

    const validate = () => {
        const newErrors = { ...errors };
        let isValid = true;

        if (!form.bedrijfsNaam.trim()) {
            newErrors.bedrijfsNaam = "Bedrijfsnaam mag niet leeg zijn.";
            isValid = false;
        }
        if (!form.contactPersoon.trim()) {
            newErrors.contactPersoon = "Contactpersoon mag niet leeg zijn.";
            isValid = false;
        }
        if (!form.email.trim() || !/^\S+@\S+\.\S+$/.test(form.email)) {
            newErrors.email = "Voer een geldig e-mailadres in.";
            isValid = false;
        }
        if (!form.telefoonNummer.trim() || !/^\+?[0-9\s\-()]{6,20}$/.test(form.telefoonNummer)) {
            newErrors.telefoonNummer = "Voer een geldig telefoonnummer in.";
            isValid = false;
        }
        if (!form.adres.trim()) {
            newErrors.adres = "Adres mag niet leeg zijn.";
            isValid = false;
        }

        setErrors(newErrors);
        return isValid;
    };

    const handleSubmit = async () => {
        if (!validate()) return;

        try {
            const response = await fetch("http://localhost:5000/api/customer", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(form),
            });

            if (response.ok) {
                onSuccess();
                onClose();
            } else {
                console.error("Fout bij opslaan klant");
            }
        } catch (err) {
            console.error("Netwerkfout:", err);
        }
    };

    return (
        <div className="modal-overlay">
            <div className="modal modal-updates klant-form">
                <button onClick={onClose} className="close-button">&times;</button>
                <h2>Klant Aanmaken</h2>

                <form
                    className="form"
                    onSubmit={(e) => {
                        e.preventDefault();
                        handleSubmit();
                    }}
                >
                    {Object.entries(form).map(([key, value]) => (
                        <div className="form-group" key={key}>
                            <label htmlFor={key}>{key}</label>
                            <input
                                id={key}
                                type="text"
                                name={key}
                                value={value}
                                onChange={handleChange}
                            />
                            {errors[key as keyof typeof errors] && (
                                <p className="error">{errors[key as keyof typeof errors]}</p>
                            )}
                        </div>
                    ))}

                    <button type="submit" className="submit-button">
                        Aanmaken
                    </button>
                </form>
            </div>
        </div>
    );
};

export default KlantAanmakenModal;
