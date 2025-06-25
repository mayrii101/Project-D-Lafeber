import React from "react";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import KlantAanmakenModal from "../components/KlantAanmakenModal";
import "@testing-library/jest-dom";

global.fetch = jest.fn(() =>
  Promise.resolve({
    ok: true,
    json: () => Promise.resolve({}),
  })
) as jest.Mock;

describe("KlantAanmakenModal", () => {
  const mockOnClose = jest.fn();
  const mockOnSuccess = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();
    render(<KlantAanmakenModal onClose={mockOnClose} onSuccess={mockOnSuccess} />);
  });

  it("shows validation errors when submitting empty form", async () => {
    fireEvent.click(screen.getByRole("button", { name: /aanmaken/i }));

    expect(await screen.findByText(/bedrijfsnaam mag niet leeg zijn/i)).toBeInTheDocument();
    expect(screen.getByText(/contactpersoon mag niet leeg zijn/i)).toBeInTheDocument();
    expect(screen.getByText(/voer een geldig e-mailadres in/i)).toBeInTheDocument();
    expect(screen.getByText(/voer een geldig telefoonnummer in/i)).toBeInTheDocument();
    expect(screen.getByText(/adres mag niet leeg zijn/i)).toBeInTheDocument();
  });

  it("submits correctly with valid input", async () => {
    fireEvent.change(screen.getByLabelText(/bedrijfsnaam/i), { target: { value: "Test BV" } });
    fireEvent.change(screen.getByLabelText(/contactpersoon/i), { target: { value: "Jan Janssen" } });
    fireEvent.change(screen.getByLabelText(/email/i), { target: { value: "test@email.com" } });
    fireEvent.change(screen.getByLabelText(/telefoonnummer/i), { target: { value: "+31612345678" } });
    fireEvent.change(screen.getByLabelText(/adres/i), { target: { value: "Straatweg 1" } });

    fireEvent.click(screen.getByRole("button", { name: /aanmaken/i }));

    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledWith(
        "http://localhost:5000/api/customer",
        expect.objectContaining({
          method: "POST",
          headers: { "Content-Type": "application/json" },
        })
      );
    });

    expect(mockOnSuccess).toHaveBeenCalled();
    expect(mockOnClose).toHaveBeenCalled();
  });

  it("does not call onSuccess if response is not OK", async () => {
    (global.fetch as jest.Mock).mockImplementationOnce(() =>
      Promise.resolve({ ok: false })
    );

    fireEvent.change(screen.getByLabelText(/bedrijfsnaam/i), { target: { value: "Test BV" } });
    fireEvent.change(screen.getByLabelText(/contactpersoon/i), { target: { value: "Jan Janssen" } });
    fireEvent.change(screen.getByLabelText(/email/i), { target: { value: "test@email.com" } });
    fireEvent.change(screen.getByLabelText(/telefoonnummer/i), { target: { value: "+31612345678" } });
    fireEvent.change(screen.getByLabelText(/adres/i), { target: { value: "Straatweg 1" } });

    fireEvent.click(screen.getByRole("button", { name: /aanmaken/i }));

    await waitFor(() => {
      expect(mockOnSuccess).not.toHaveBeenCalled();
      expect(mockOnClose).not.toHaveBeenCalled();
    });
  });
});
